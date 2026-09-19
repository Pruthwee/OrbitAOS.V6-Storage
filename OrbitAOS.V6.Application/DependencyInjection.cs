using Microsoft.Extensions.DependencyInjection;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Application.Services;

namespace OrbitAOS.V6.Application;

/// <summary>
/// Extension methods for registering Application layer services with the DI container.
/// Call this from Program.cs to wire up all application services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Application layer services into the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserProfileService, UserProfileService>();
        return services;
    }
}
