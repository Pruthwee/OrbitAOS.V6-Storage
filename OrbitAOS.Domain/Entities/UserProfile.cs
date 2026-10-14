using OrbitAOS.Domain.Common;

namespace OrbitAOS.Domain.Entities;

/// <summary>
/// Represents an OrbitAOS user profile entity in the domain layer.
/// Extends the base entity with user-specific properties.
/// </summary>
public class UserProfile : BaseEntity, IAggregateRoot
{
    public string IdentityUserId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Position { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
}
