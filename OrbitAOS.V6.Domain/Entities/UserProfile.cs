using OrbitAOS.V6.Domain.Common;

namespace OrbitAOS.V6.Domain.Entities;

/// <summary>
/// Represents an application user profile linked to ASP.NET Core Identity.
/// Extends the base entity with user-specific domain properties.
/// </summary>
public class UserProfile : BaseEntity, IAggregateRoot
{
    /// <summary>Gets or sets the ASP.NET Core Identity user ID (foreign key).</summary>
    public string IdentityUserId { get; set; } = string.Empty;

    /// <summary>Gets or sets the display name of the user.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets whether the user profile is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Gets or sets the date the user last logged in.</summary>
    public DateTime? LastLoginAt { get; set; }
}
