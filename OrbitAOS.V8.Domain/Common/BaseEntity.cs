namespace OrbitAOS.V8.Domain.Common;

/// <summary>
/// Base entity class providing common audit fields for all domain entities.
/// Replaces legacy model base classes that relied on System.Web or EF6 conventions.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Primary key identifier.</summary>
    public int Id { get; set; }

    /// <summary>UTC timestamp when the entity was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp when the entity was last updated.</summary>
    public DateTime? UpdatedAt { get; set; }
}
