using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrbitAOS.V6.Domain.Interfaces;
using OrbitAOS.V6.Infrastructure.Data;
using OrbitAOS.V6.Infrastructure.Repositories;

namespace OrbitAOS.V6.Infrastructure;

/// <summary>
/// Extension methods for registering Infrastructure layer services with the DI container.
/// Call this from Program.cs to wire up EF Core, Identity, and repository implementations.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Infrastructure layer services including EF Core DbContext,
    /// ASP.NET Core Identity core services, and repository implementations.
    /// Note: AddDefaultIdentity (which includes Identity UI) must be called from the
    /// Web layer (Program.cs) because it requires the Microsoft.AspNetCore.Identity.UI package.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration (for connection strings).</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register EF Core DbContext with SQL Server provider
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Register EF Core developer exception filter (shows migration errors in dev)
        services.AddDatabaseDeveloperPageExceptionFilter();

        // Register repository implementations
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();

        return services;
    }
}
