// ============================================================
// Infrastructure Layer - ApplicationDbContext
// Upgraded from net6.0 to net8.0 EF Core.
// ============================================================
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OrbitAOS.V6.Data
{
    /// <summary>
    /// Application database context.
    /// Inherits from <see cref="IdentityDbContext"/> to include ASP.NET Core Identity tables.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ApplicationDbContext"/>.
        /// </summary>
        /// <param name="options">EF Core options configured in Program.cs.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Add domain DbSet properties here as the application grows.
        // Example:
        // public DbSet<Product> Products => Set<Product>();

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply all entity configurations from the current assembly.
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
