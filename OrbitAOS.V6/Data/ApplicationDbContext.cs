using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OrbitAOS.V6.Data;

/// <summary>
/// Application database context for ASP.NET Core Identity.
/// Updated from EF Core 6.0 to EF Core 8.0 for .NET 8 compatibility.
/// </summary>
public class ApplicationDbContext : IdentityDbContext
{
    /// <summary>Initializes a new instance of ApplicationDbContext.</summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}
