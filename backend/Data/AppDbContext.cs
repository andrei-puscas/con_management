using Microsoft.EntityFrameworkCore;
using Backend.Entities;

namespace Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Utilizator> Utilizatori { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Utilizator>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
        });
    }
}
