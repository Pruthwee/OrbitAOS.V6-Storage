using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OrbitAOS.V6.Data;

/// <summary>
/// Application database context for OrbitAOS.V6.
/// Migrated from net6.0 to net10.0.
/// Extends IdentityDbContext to include ASP.NET Core Identity tables.
/// </summary>
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}
