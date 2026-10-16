// ============================================================
// Infrastructure/Services/CurrentUserService.cs
// Clean Architecture - Infrastructure Layer
// Implements ICurrentUserService using IHttpContextAccessor
// Replaces: HttpContext.Current.User (System.Web)
// ============================================================

using OrbitAOS.V6.Application.Common.Interfaces;
using System.Security.Claims;

namespace OrbitAOS.V6.Infrastructure.Services;

/// <summary>
/// Concrete implementation of ICurrentUserService using ASP.NET Core's
/// IHttpContextAccessor. This replaces the legacy System.Web.HttpContext.Current
/// pattern which is not available in ASP.NET Core.
///
/// Legacy pattern (REMOVED - System.Web not available in .NET 10):
///   var userId = HttpContext.Current.User.Identity.Name;
///
/// .NET 10 pattern (this implementation):
///   Inject ICurrentUserService and use UserId/UserName properties.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc/>
    public string? UserId =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    /// <inheritdoc/>
    public string? UserName =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name)
        ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

    /// <inheritdoc/>
    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    /// <inheritdoc/>
    public bool IsInRole(string role) =>
        _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
}
