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
            var userDescending =  await _context.User.OrderByDescending(u => u.Points).ToListAsync();
            
            for (var i = 0; i < userDescending.Count; i++)
            {
                userDescending[i].Rank = (uint)(i + 1);
            }

            return userDescending;
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, UserUpdate update)
        {
            if (id != update.Id || !UserExists(id))
            {
                return BadRequest();
            }

            var user = await _context.User.FindAsync(id);
            user.Points = update.Points;
            
            _context.Entry(user).State = EntityState.Modified;
            
            await _context.SaveChangesAsync();
            await _rankingService.UpdateRanks();
 
            return NoContent();
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(string nickname)
        {
            // Could use a profanity filter
            if (string.IsNullOrEmpty(nickname) || nickname.Length > 20 || nickname.Length < 3)
            {
                return BadRequest("Nickname must be between 3 and 20 characters");
            }
            
            // Check if nickname is taken
            if (_context.User.Any(u => u.NickName == nickname))
            {
                return BadRequest("Nickname is already taken");
            }
            
            var user = new User { NickName = nickname }; 
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            // Update all ranks
            await _rankingService.UpdateRanks();
            user = await _context.User.FindAsync(user.Id);

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            
            await _rankingService.UpdateRanks();

            return NoContent();
        }

        private bool UserExists(long id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
