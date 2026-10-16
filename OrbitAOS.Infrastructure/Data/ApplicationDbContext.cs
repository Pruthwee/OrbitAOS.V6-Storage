using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Entities;

namespace OrbitAOS.Infrastructure.Data;

/// <summary>
/// Application database context combining ASP.NET Core Identity with domain entities.
/// Inherits from IdentityDbContext to include all Identity tables.
/// </summary>
public class ApplicationDbContext : IdentityDbContext
{
    /// <summary>Initializes a new instance of ApplicationDbContext.</summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>Gets or sets the UserProfiles DbSet for domain user profile data.</summary>
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure UserProfile entity
        builder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IdentityUserId).IsRequired().HasMaxLength(450);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.UpdatedBy).HasMaxLength(256);
            entity.HasIndex(e => e.IdentityUserId).IsUnique();
            entity.HasIndex(e => e.Email);
        });
    }
}
