// ============================================================
// Domain/Entities/BaseEntity.cs
// Clean Architecture - Domain Layer
// Base entity with audit fields for all domain entities
// ============================================================

namespace OrbitAOS.V6.Domain.Entities;

/// <summary>
/// Base entity providing common audit fields for all domain entities.
/// Implements the audit trail pattern for .NET 10 Clean Architecture.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Primary key identifier.</summary>
    public int Id { get; set; }

    /// <summary>UTC timestamp when the entity was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>User identifier who created the entity.</summary>
    public string? CreatedBy { get; set; }

    /// <summary>UTC timestamp when the entity was last modified.</summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>User identifier who last modified the entity.</summary>
    public string? UpdatedBy { get; set; }

    /// <summary>Soft-delete flag (replaces physical DELETE operations).</summary>
    public bool IsDeleted { get; set; } = false;
}
