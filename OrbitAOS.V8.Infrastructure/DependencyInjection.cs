using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrbitAOS.V8.Domain.Interfaces;
using OrbitAOS.V8.Infrastructure.Data;
using OrbitAOS.V8.Infrastructure.Identity;
using OrbitAOS.V8.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace OrbitAOS.V8.Infrastructure;

/// <summary>
/// Extension methods for registering Infrastructure layer services with the DI container.
/// 
/// Migration notes from legacy ASP.NET MVC 6.0:
/// - Replaces Global.asax Application_Start() database initialization
/// - Replaces web.config connectionStrings with appsettings.json
/// - Replaces Database.SetInitializer() with EF Core Migrations
/// - Replaces ASP.NET Identity 2.x configuration with ASP.NET Core Identity
/// - No more DependencyResolver.SetResolver() — uses built-in DI
/// 
/// Usage in Program.cs:
///   builder.Services.AddInfrastructureServices(builder.Configuration);
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Infrastructure layer services including EF Core, Identity, and repositories.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // EF Core DbContext — replaces EF6 DbContext with Database.SetInitializer
        // Connection string from appsettings.json (replaces web.config connectionStrings)
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    // Enable retry on transient failures (not available in EF6)
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                }));

        // Developer exception page for EF Core migrations (Development only)
        services.AddDatabaseDeveloperPageExceptionFilter();

        // ASP.NET Core Identity — replaces ASP.NET Identity 2.x
        // Migration note: RequireConfirmedAccount = false for easier migration;
        // set to true in production after email confirmation is configured.
        services.AddDefaultIdentity<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;

            // Password policy — matches or exceeds legacy MVC 6.0 defaults
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

        // Repository registrations — replaces direct DbContext injection in controllers
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();

        return services;
    }
}
