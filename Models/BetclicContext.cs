using Microsoft.EntityFrameworkCore;

namespace BetclicApi.Models;

public class BetclicContext : DbContext
{
    public BetclicContext(DbContextOptions<BetclicContext> options)
        : base(options)
    {
    }

    public DbSet<User> User { get; set; } = null!;
}