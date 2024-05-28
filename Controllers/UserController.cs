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

        /// <summary>
        /// Retrieves all users, ordered by points in descending order, and assigns ranks based on
        /// their points.
        /// </summary>
        /// <returns>A list of users with their respective ranks.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
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

        /// <summary>
        /// Retrieves a specific user by their ID, along with their computed rank.
        /// </summary>
        /// <param name="id">The ID of the user to be retrieved.</param>
        /// <returns>
        /// The user with the specified ID, or a NotFound result if the user does not exist.
        /// </returns>
        /// <response code="200">Returns the user with the specified ID.</response>
        /// <response code="404">If the user with the specified ID is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Updates the points of an existing user.
        /// </summary>
        /// <param name="id">The ID of the user to be updated.</param>
        /// <param name="update">An object containing the new points for the user.</param>
        /// <returns>
        /// A NoContent result if the update is successful, or a NotFound result if the user does
        /// not exist.
        /// </returns>
        /// <response code="204">NoContent: The update is successful.</response>
        /// <response code="404">NotFound: The user does not exist.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Creates a new user with the specified username.
        /// </summary>
        /// <param name="creation">An object containing the username for the new user.</param>
        /// <returns>The created user, or a BadRequest result if the username is already in use
        /// or other validation errors occur.</returns>
        /// <response code="201">The created user.</response>
        /// <response code="400">Bad request result if the username is already in use or other validation errors occur.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Deletes all users from the database.
        /// </summary>
        /// <returns>A NoContent result indicating the deletion was successful.</returns>
        /// <response code="204">NoContent. The deletion was successful.</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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
