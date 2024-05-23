using Microsoft.EntityFrameworkCore;

namespace BetclicApi.Models;

public class BetclicContext : DbContext
{
    public DbSet<User> User { get; set; } = null!;
    public string DbPath { get; }

    public BetclicContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "user_data.db");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.NickName).IsUnique();
    
        base.OnModelCreating(modelBuilder);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}