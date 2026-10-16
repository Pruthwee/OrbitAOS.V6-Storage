using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Entities;

namespace OrbitAOS.Infrastructure.Data;

/// <summary>
/// EF Core DbContext for the OrbitAOS application.
/// Inherits from IdentityDbContext to include ASP.NET Core Identity tables.
/// Migrated from EF Core 3.0 (net6.0) to EF Core 8.0 (net8.0).
/// </summary>
public class ApplicationDbContext : IdentityDbContext
{
    /// <summary>Initializes a new instance of <see cref="ApplicationDbContext"/>.</summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>Gets or sets the UserProfiles DbSet.</summary>
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
            entity.HasIndex(e => e.IdentityUserId).IsUnique();
        });
    }
}
