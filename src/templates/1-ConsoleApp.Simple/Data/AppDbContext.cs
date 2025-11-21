//#if (UseEfCore)
using ConsoleApp.Simple.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp.Simple.Data;

/// <summary>
/// Entity Framework Core database context.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ExampleEntity> Examples => Set<ExampleEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ExampleEntity>(entity =>
        {
            entity.ToTable("Examples");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CreatedAt).IsRequired();
        });
    }
}
//#endif
