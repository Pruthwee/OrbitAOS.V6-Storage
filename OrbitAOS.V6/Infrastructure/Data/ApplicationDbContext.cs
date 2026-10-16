// ============================================================
// Infrastructure/Data/ApplicationDbContext.cs
// Clean Architecture - Infrastructure Layer
// EF Core 10 DbContext replacing EF6 / net6.0 DbContext
// Replaces: Data/ApplicationDbContext.cs (net6.0 version)
// ============================================================

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Application.Common.Interfaces;
using OrbitAOS.V6.Domain.Entities;
using System.Reflection;

namespace OrbitAOS.V6.Infrastructure.Data;

/// <summary>
/// EF Core 10 application database context.
/// Extends IdentityDbContext&lt;ApplicationUser&gt; for ASP.NET Core Identity.
/// 
/// Migration from net6.0:
/// - TargetFramework: net6.0 → net10.0
/// - EF Core packages: 6.0.19 → 10.x
/// - ApplicationUser: IdentityUser → custom ApplicationUser entity
/// - Added IApplicationDbContext interface for testability
/// - Added audit field interception via SaveChangesAsync override
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // -------------------------------------------------------
    // Add DbSet<T> properties for each domain entity here
    // Example: public DbSet<Product> Products => Set<Product>();
    // -------------------------------------------------------

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all entity configurations from the Infrastructure assembly
        // This replaces individual fluent API calls scattered in OnModelCreating
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Customize Identity table names if needed (optional)
        // builder.Entity<ApplicationUser>().ToTable("Users");
        // builder.Entity<IdentityRole>().ToTable("Roles");

        // Global query filter for soft-delete (optional)
        // builder.Entity<BaseEntity>().HasQueryFilter(e => !e.IsDeleted);
    }

    /// <summary>
    /// Overrides SaveChangesAsync to automatically set audit fields.
    /// Replaces: manual CreatedAt/UpdatedAt assignment in legacy repositories.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-set audit fields on BaseEntity-derived entities
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
