using Microsoft.EntityFrameworkCore;
using VertrauApi.Models;

namespace VertrauApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Sobrenome).IsRequired().HasMaxLength(100);
            
            entity.Property(e => e.Email).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.Property(e => e.Genero).IsRequired();
            // DataNascimento is nullable, no specific config needed beyond mapping
        });
    }
}
