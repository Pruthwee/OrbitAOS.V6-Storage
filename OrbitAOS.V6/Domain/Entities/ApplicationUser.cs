// ============================================================
// Domain/Entities/ApplicationUser.cs
// Clean Architecture - Domain Layer
// Extends IdentityUser for ASP.NET Core Identity on .NET 10
// Replaces: ASP.NET Identity 2.x ApplicationUser
// ============================================================

using Microsoft.AspNetCore.Identity;

namespace OrbitAOS.V6.Domain.Entities;

/// <summary>
/// Application user entity extending ASP.NET Core Identity's IdentityUser.
/// Migrated from ASP.NET Identity 2.x (MVC 5/6) to ASP.NET Core Identity (.NET 10).
/// The Identity database schema (AspNetUsers, AspNetRoles, etc.) remains compatible.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>User's first name.</summary>
    public string? FirstName { get; set; }

    /// <summary>User's last name.</summary>
    public string? LastName { get; set; }

    /// <summary>Full display name computed from first and last name.</summary>
    public string DisplayName => $"{FirstName} {LastName}".Trim();

    /// <summary>UTC timestamp when the user account was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp of the user's last login.</summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>Indicates whether the user account is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Profile picture URL or path.</summary>
    public string? ProfilePictureUrl { get; set; }
}
