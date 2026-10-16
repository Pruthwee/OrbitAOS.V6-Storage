using Microsoft.Extensions.DependencyInjection;
using OrbitAOS.V8.Application.Interfaces;
using OrbitAOS.V8.Application.Services;

namespace OrbitAOS.V8.Application;

/// <summary>
/// Extension methods for registering Application layer services with the DI container.
/// Migration note: In legacy MVC 6.0, services were registered in Global.asax or
/// via DependencyResolver. In .NET 8, this is done via IServiceCollection extensions
/// called from Program.cs.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Application layer services.
    /// Call this from Program.cs: builder.Services.AddApplicationServices();
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IAuditLogService, AuditLogService>();

        return services;
    }
}
