// ============================================================
// Application/Common/Interfaces/ICurrentUserService.cs
// Clean Architecture - Application Layer
// Replaces: HttpContext.Current.User (System.Web)
// ============================================================

namespace OrbitAOS.V6.Application.Common.Interfaces;

/// <summary>
/// Service interface for accessing the currently authenticated user.
/// Replaces the legacy System.Web HttpContext.Current.User pattern.
/// In ASP.NET Core, use IHttpContextAccessor injected via DI.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>Gets the current user's unique identifier (sub claim).</summary>
    string? UserId { get; }

    /// <summary>Gets the current user's username/email.</summary>
    string? UserName { get; }

    /// <summary>Indicates whether the current request is authenticated.</summary>
    bool IsAuthenticated { get; }

    /// <summary>Checks whether the current user is in the specified role.</summary>
    bool IsInRole(string role);
}
