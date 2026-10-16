using OrbitAOS.V8.Domain.Common;

namespace OrbitAOS.V8.Domain.Entities;

/// <summary>
/// Represents an application user profile entity in the domain layer.
/// Decoupled from ASP.NET Core Identity — Identity user is in Infrastructure.
/// Migration note: In legacy MVC 6.0, ApplicationUser extended IdentityUser directly
/// in the Models folder. In Clean Architecture, domain entities are identity-agnostic.
/// </summary>
public class UserProfile : BaseEntity
{
    /// <summary>Reference to the ASP.NET Core Identity user ID (string GUID).</summary>
    public string IdentityUserId { get; set; } = string.Empty;

    /// <summary>Display name for the user.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>User's email address (mirrored from Identity for query convenience).</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Indicates whether the user profile is active.</summary>
    public bool IsActive { get; set; } = true;
}
