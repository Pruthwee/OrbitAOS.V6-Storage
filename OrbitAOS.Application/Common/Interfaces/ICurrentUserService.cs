namespace OrbitAOS.Application.Common.Interfaces;

/// <summary>
/// Abstraction for accessing the current user's identity information within the application layer.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>Gets the current user's identity ID (ASP.NET Core Identity user ID).</summary>
    string? UserId { get; }

    /// <summary>Gets the current user's username or email.</summary>
    string? UserName { get; }

    /// <summary>Gets whether the current request is authenticated.</summary>
    bool IsAuthenticated { get; }
}
