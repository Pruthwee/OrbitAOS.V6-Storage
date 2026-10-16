using Microsoft.AspNetCore.Http;
using OrbitAOS.Application.Common.Interfaces;

namespace OrbitAOS.Infrastructure.Identity;

/// <summary>
/// Implementation of ICurrentUserService that reads the current user from the HTTP context.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>Initializes a new instance of CurrentUserService.</summary>
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public string? UserId =>
        _httpContextAccessor.HttpContext?.User?.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

    /// <inheritdoc />
    public string? UserName =>
        _httpContextAccessor.HttpContext?.User?.Identity?.Name;

    /// <inheritdoc />
    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
