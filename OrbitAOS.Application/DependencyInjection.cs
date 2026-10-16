using Microsoft.Extensions.DependencyInjection;
using OrbitAOS.Application.Interfaces;
using OrbitAOS.Application.Services;

namespace OrbitAOS.Application
{
    /// <summary>
    /// Extension methods for registering Application layer services with the DI container.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register application services
            // Note: IUserService implementation is registered in Infrastructure layer
            // UserService wraps the infrastructure implementation with logging
            services.AddScoped<UserService>();

            return services;
        }
    }
}
