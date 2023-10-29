using Microsoft.EntityFrameworkCore;
using PublisherBot.Models;
namespace PublisherBot.Data;


public class ApplicationDbContext : DbContext
{
    public DbSet<Models.Post> Posts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = Configuration.Configuration.FromConfig();
        optionsBuilder.UseNpgsql(config?.database);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<MyUser>().ToTable("TelegramUsers");
        modelBuilder.Entity<Transaction>().ToTable("Transactions");
    }

}


