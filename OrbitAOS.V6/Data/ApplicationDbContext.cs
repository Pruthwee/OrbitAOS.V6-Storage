using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OrbitAOS.V6.Data;

/// <summary>
/// EF Core DbContext for OrbitAOS.V6.
/// Upgraded from EF Core 6.0 (net6.0) to EF Core 8.0 (net8.0).
/// Inherits from IdentityDbContext to include ASP.NET Core Identity tables.
/// </summary>
public class ApplicationDbContext : IdentityDbContext
{
    /// <summary>Initializes a new instance of <see cref="ApplicationDbContext"/>.</summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}
