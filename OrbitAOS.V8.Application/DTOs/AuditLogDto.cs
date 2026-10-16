namespace OrbitAOS.V8.Application.DTOs;

/// <summary>
/// Data Transfer Object for audit log entries.
/// Migration note: Replaces ad-hoc logging ViewModels from legacy MVC 6.0.
/// </summary>
public class AuditLogDto
{
    public int Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? UserId { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }
}
