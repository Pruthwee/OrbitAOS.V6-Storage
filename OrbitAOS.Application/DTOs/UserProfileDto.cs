namespace OrbitAOS.Application.DTOs;

/// <summary>
/// Data Transfer Object for user profile information.
/// Used to transfer user data between application layers.
/// </summary>
public class UserProfileDto
{
    public int Id { get; set; }
    public string IdentityUserId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Position { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
