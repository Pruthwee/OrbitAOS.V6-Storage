using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V8.Domain.Entities;
using OrbitAOS.V8.Infrastructure.Identity;

namespace OrbitAOS.V8.Infrastructure.Data;

/// <summary>
/// Application database context for .NET 8 ASP.NET Core MVC.
/// 
/// Migration notes from legacy ASP.NET MVC 6.0:
/// - Inherits from IdentityDbContext&lt;ApplicationUser&gt; instead of IdentityDbContext (EF6)
/// - Uses EF Core 8.0 instead of Entity Framework 6.x
/// - Connection string comes from appsettings.json (not web.config)
/// - Configured in Program.cs via builder.Services.AddDbContext&lt;T&gt;()
/// - No more Database.SetInitializer() — use EF Core Migrations
/// - Nullable reference types enabled throughout
/// 
/// Breaking changes from EF6:
/// - No lazy loading by default (use .Include() for eager loading)
/// - No ObjectContext — use DbContext only
/// - No Database.ExecuteSqlCommand() — use ExecuteSqlRaw() or ExecuteSqlInterpolated()
/// - No EntityState.Unchanged manipulation — use ChangeTracker
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Domain entity DbSets
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // UserProfile configuration
        builder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IdentityUserId).IsRequired().HasMaxLength(450);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.HasIndex(e => e.IdentityUserId).IsUnique();
            entity.HasIndex(e => e.Email);
        });

        // AuditLog configuration
        builder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(128);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(256);
            entity.Property(e => e.EntityId).HasMaxLength(450);
            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.EntityType);
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}
