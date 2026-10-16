using OrbitAOS.V8.Domain.Common;

namespace OrbitAOS.V8.Domain.Entities;

/// <summary>
/// Represents an audit log entry for tracking application events.
/// Migration note: Legacy MVC 6.0 applications often used custom logging tables.
/// This entity provides a domain-level audit trail.
/// </summary>
public class AuditLog : BaseEntity
{
    /// <summary>The action performed (e.g., "Login", "Create", "Update", "Delete").</summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>The entity type affected by the action.</summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>The ID of the affected entity (if applicable).</summary>
    public string? EntityId { get; set; }

    /// <summary>The user who performed the action.</summary>
    public string? UserId { get; set; }

    /// <summary>Additional details or serialized data about the action.</summary>
    public string? Details { get; set; }

    /// <summary>IP address of the request originator.</summary>
    public string? IpAddress { get; set; }
}
