using Microsoft.EntityFrameworkCore;

namespace BetclicApi.Models;

/// <summary>
/// The BetclicContext class represents the database context for the application, 
/// providing access to the User table and configuring the database connection and model.
/// </summary>
public class BetclicContext : DbContext
{
    /// <summary>
    /// Gets or sets the DbSet for the User entity, which allows CRUD operations on the User table.
    /// </summary>
    public DbSet<User> User { get; set; } = null!;

    /// <summary>
    /// Gets the path to the SQLite database file.
    /// </summary>
    public string DbPath { get; }

    /// <summary>
    /// Initializes a new instance of the BetclicContext class, setting up the path to the SQLite
    /// database file.
    /// </summary>
    public BetclicContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "user_data.db");
    }

    /// <summary>
    /// Configures the model and its relationships. In this context, it ensures that the Username
    /// property of the User entity is unique.
    /// </summary>
    /// <param name="modelBuilder">The ModelBuilder used to configure the model.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Configures the database options, specifically setting the SQLite database connection
    /// string.
    /// </summary>
    /// <param name="options">
    /// The DbContextOptionsBuilder used to configure the database options.
    /// </param>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}