using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OrbitAOS.V6.Data
{
    /// <summary>
    /// Application database context for OrbitAOS.V6.
    /// Updated for EF Core 10 compatibility.
    /// Migration from EF Core 6.0.19 to EF Core 10.0.0:
    ///   - IdentityDbContext now uses generic IdentityUser (no breaking change)
    ///   - DbContextOptions constructor pattern unchanged
    ///   - OnModelCreating base call required for Identity schema
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Add custom entity configurations here
        }
    }
}
