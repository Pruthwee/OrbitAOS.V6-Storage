using Microsoft.AspNetCore.Identity;

namespace OrbitAOS.V8.Infrastructure.Identity;

/// <summary>
/// Application user extending ASP.NET Core Identity's IdentityUser.
/// 
/// Migration notes from legacy ASP.NET MVC 6.0:
/// - Replaces ApplicationUser that extended Microsoft.AspNet.Identity.EntityFramework.IdentityUser (EF6)
/// - Now extends Microsoft.AspNetCore.Identity.IdentityUser (ASP.NET Core Identity)
/// - No more UserManager&lt;ApplicationUser, string&gt; — use UserManager&lt;ApplicationUser&gt;
/// - No more IUser interface — use IdentityUser directly
/// - Database schema is compatible with ASP.NET Core Identity (AspNetUsers table)
/// 
/// Key differences from ASP.NET Identity 2.x:
/// - PasswordHasher algorithm updated (PBKDF2 with SHA-256, 10000 iterations)
/// - SecurityStamp validation on every request (configurable)
/// - Two-factor authentication built-in
/// - Lockout enabled by default
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>User's full display name.</summary>
    public string? DisplayName { get; set; }

    /// <summary>UTC timestamp when the user registered.</summary>
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp of the user's last login.</summary>
    public DateTime? LastLoginAt { get; set; }
}
