using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetclicApi.Models;

namespace BetclicApi.Controllers
{
    /// <summary>
    /// Controller for managing User entities via API endpoints.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BetclicContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class with the specified
        /// context.
        /// </summary>
        /// <param name="context">The database context to be used.</param>
        public UserController(BetclicContext context)
        {
            _context = context;
        }

        // GET: api/User
        /// <summary>
        /// Retrieves all users, ordered by points in descending order, and assigns ranks based on
        /// their points.
        /// </summary>
        /// <returns>A list of users with their respective ranks.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {   
            var rankedUsers = _context.User
                .AsEnumerable()
                .OrderByDescending(u => u.Points)
                .Select((u, index) => new User {
                    Id = u.Id, 
                    Username = u.Username,
                    Points = u.Points,
                    Rank = (uint) index + 1,
                })
                .ToList();

            return rankedUsers;
        }

        // GET: api/User/{id}
        /// <summary>
        /// Retrieves a specific user by their ID, along with their computed rank.
        /// </summary>
        /// <param name="id">The ID of the user to be retrieved.</param>
        /// <returns>
        /// The user with the specified ID, or a NotFound result if the user does not exist.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
            // Need to compute all ranks
            var user = _context.User
                .AsEnumerable()
                .OrderByDescending(u => u.Points)
                .Select((u, index) => new User
                {
                    Id = u.Id,
                    Username = u.Username,
                    Points = u.Points,
                    Rank = (uint)index + 1,
                }).FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/User/{id}
        /// <summary>
        /// Updates the points of an existing user.
        /// </summary>
        /// <param name="id">The ID of the user to be updated.</param>
        /// <param name="update">An object containing the new points for the user.</param>
        /// <returns>
        /// A NoContent result if the update is successful, or a NotFound result if the user does
        /// not exist.
        /// </returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, UserUpdate update)
        {
            if (!UserExists(id))
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            user.Points = update.Points;
            
            _context.Entry(user).State = EntityState.Modified;
            
            await _context.SaveChangesAsync();
 
            return NoContent();
        }

        // POST: api/User
        /// <summary>
        /// Creates a new user with the specified username.
        /// </summary>
        /// <param name="creation">An object containing the username for the new user.</param>
        /// <returns>The created user, or a BadRequest result if the username is already in use
        /// or other validation errors occur.</returns>
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserCreation creation)
        {
            var user = new User { Username = creation.Username }; 

            // Could use a profanity filter
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // Check if username is taken
            try
            {
                _context.User.Add(user);
                await _context.SaveChangesAsync();

                // TODO: Explain in the README why I do that
                // Need to compute all ranks
                user = _context.User
                    .AsEnumerable()
                    .OrderByDescending(u => u.Points)
                    .Select((u, index) => new User
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Points = u.Points,
                        Rank = (uint)index + 1,
                    }).FirstOrDefault(u => u.Id == user.Id);

                return CreatedAtAction("GetUser", new { id = user.Id }, user);
            }
            catch (DbUpdateException ex)
            {
                var errorResponse = new
                {
                    type = "Database Exception",
                    title = "Username unicity exception",
                    status = 400,
                    errors = new Dictionary<string, string[]>
                    {
                        { "Username", ["This Username is already in use, please try another one."] }
                    },
                    traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                };
                return BadRequest(errorResponse);
            }
        }

        // DELETE: api/User
        /// <summary>
        /// Deletes all users from the database.
        /// </summary>
        /// <returns>A NoContent result indicating the deletion was successful.</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            _context.User.RemoveRange(await _context.User.ToListAsync());
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        /// <summary>
        /// Checks if a user with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the user to check for existence.</param>
        /// <returns>True if the user exists, otherwise false.</returns>
        private bool UserExists(long id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
