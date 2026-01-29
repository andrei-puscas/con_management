using Microsoft.EntityFrameworkCore;
using Backend.Entities;

namespace Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Utilizator> Utilizatori { get; set; }
    public DbSet<Proiect> Proiecte { get; set; }
    public DbSet<Santier> Santier { get; set; }
    public DbSet<Echipa> Echipe { get; set; }
    public DbSet<Angajat> Angajati { get; set; }
    public DbSet<Lucrare> Lucrari { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Utilizator>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.HasOne(u => u.Angajat)
                .WithOne(a => a.Utilizator)
                .HasForeignKey<Utilizator>(u => u.AngajatId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Santier>(e =>
        {
            e.HasOne(s => s.Proiect)
                .WithMany(p => p.Santier)
                .HasForeignKey(s => s.ProiectId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Angajat>(e =>
        {
            e.HasOne(a => a.Echipa)
                .WithMany(ec => ec.Angajati)
                .HasForeignKey(a => a.EchipaId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Echipa>(e =>
        {
            e.HasOne(ec => ec.SefEchipa)
                .WithMany()
                .HasForeignKey(ec => ec.SefEchipaId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Lucrare>(e =>
        {
            e.HasOne(l => l.Santier)
                .WithMany(s => s.Lucrari)
                .HasForeignKey(l => l.SantierId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Lucrare>()
            .HasMany(l => l.Echipe)
            .WithMany(e => e.Lucrari)
            .UsingEntity(j => j.ToTable("LucrareEchipa"));
    }
}
