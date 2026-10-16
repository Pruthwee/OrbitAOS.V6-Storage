namespace OrbitAOS.V8.Application.DTOs;

/// <summary>
/// Data Transfer Object for user profile information.
/// Migration note: Replaces ViewModels that were tightly coupled to Identity in legacy MVC 6.0.
/// In Clean Architecture, DTOs are defined in Application and mapped from/to domain entities.
/// </summary>
public class UserProfileDto
{
    public int Id { get; set; }
    public string IdentityUserId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
