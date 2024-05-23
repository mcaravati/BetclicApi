using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetclicApi.Models;
using BetclicApi.Services;

namespace BetclicApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BetclicContext _context;
        private readonly RankingService _rankingService;

        public UserController(BetclicContext context)
        {
            _context = context;
            _rankingService = new RankingService(_context);
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {   
            var rankedUsers = _context.User
                .AsEnumerable()
                .OrderByDescending(u => u.Points)
                .Select((u, index) => new User {
                    Id = u.Id, 
                    NickName = u.NickName,
                    Points = u.Points,
                    Rank = (uint) index + 1,
                })
                .ToList();

            return rankedUsers;
        }

        // GET: api/User/5
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
                    NickName = u.NickName,
                    Points = u.Points,
                    Rank = (uint)index + 1,
                }).FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/User/5
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
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserCreation creation)
        {
            var user = new User { NickName = creation.NickName }; 

            // Could use a profanity filter
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // Check if nickname is taken
            try
            {
                _context.User.Add(user);
                await _context.SaveChangesAsync();

                // Update all ranks
                await _rankingService.UpdateRanks();

                // TODO: Explain in the README why I do that
                // Need to compute all ranks
                user = _context.User
                    .AsEnumerable()
                    .OrderByDescending(u => u.Points)
                    .Select((u, index) => new User
                    {
                        Id = u.Id,
                        NickName = u.NickName,
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
                    title = "Nickname unicity exception",
                    status = 400,
                    errors = new Dictionary<string, string[]>
                    {
                        { "NickName", ["This nickname is already in use, please try another one."] }
                    },
                    traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                };
                return BadRequest(errorResponse);
            }
        }

        // DELETE: api/User
        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            _context.User.RemoveRange(await _context.User.ToListAsync());
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        private bool UserExists(long id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
