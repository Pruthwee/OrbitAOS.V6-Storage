// ============================================================
// Web/ViewModels/UserViewModel.cs
// Clean Architecture - Web Layer
// ViewModel for user display/management
// ============================================================

namespace OrbitAOS.V6.Web.ViewModels;

/// <summary>
/// ViewModel for displaying user information in views.
/// Decouples the view from the domain entity (ApplicationUser).
/// AutoMapper maps ApplicationUser → UserViewModel via MappingProfile.
/// </summary>
public class UserViewModel
{
    /// <summary>User's unique identifier.</summary>
    public string? Id { get; set; }

    /// <summary>User's full display name.</summary>
    public string? FullName { get; set; }

    /// <summary>User's email address.</summary>
    public string? Email { get; set; }

    /// <summary>Indicates whether the user account is active.</summary>
    public bool IsActive { get; set; }

    /// <summary>User's assigned roles.</summary>
    public IList<string> Roles { get; set; } = new List<string>();

    /// <summary>UTC timestamp of the user's last login.</summary>
    public DateTime? LastLoginAt { get; set; }
}
