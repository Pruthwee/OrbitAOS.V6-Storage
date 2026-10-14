namespace OrbitAOS.Domain.Common;

/// <summary>
/// Base class for all domain entities providing common audit fields.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; protected set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
