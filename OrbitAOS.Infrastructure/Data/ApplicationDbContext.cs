using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Entities;

namespace OrbitAOS.Infrastructure.Data;

/// <summary>
/// Application database context for OrbitAOS.
/// Extends IdentityDbContext to include ASP.NET Core Identity tables
/// alongside application-specific domain entities.
/// Migrated from net6.0 to net10.0 with updated EF Core APIs.
/// </summary>
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// User profile domain entities stored in the database.
    /// </summary>
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure UserProfile entity
        builder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IdentityUserId)
                  .IsRequired()
                  .HasMaxLength(450);
            entity.Property(e => e.DisplayName)
                  .IsRequired()
                  .HasMaxLength(256);
            entity.Property(e => e.Department)
                  .HasMaxLength(256);
            entity.Property(e => e.Position)
                  .HasMaxLength(256);
            entity.HasIndex(e => e.IdentityUserId)
                  .IsUnique();
        });
    }
}
