using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Entities;

namespace OrbitAOS.Infrastructure.Data;

/// <summary>
/// Application database context combining ASP.NET Core Identity tables
/// with application-specific domain entities.
/// Migrated from .NET 6 IdentityDbContext to .NET 8 compatible version.
/// </summary>
public class ApplicationDbContext : IdentityDbContext
{
    /// <summary>
    /// Initializes a new instance of <see cref="ApplicationDbContext"/>.
    /// </summary>
    /// <param name="options">The DbContext options configured in Program.cs.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>Gets or sets the user profiles DbSet.</summary>
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    /// <inheritdoc/>
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
            entity.Property(e => e.Email)
                  .IsRequired()
                  .HasMaxLength(256);
            entity.HasIndex(e => e.IdentityUserId)
                  .IsUnique();
            entity.HasIndex(e => e.Email);
        });
    }
}
