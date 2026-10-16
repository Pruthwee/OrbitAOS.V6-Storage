using OrbitAOS.V8.Application.DTOs;

namespace OrbitAOS.V8.Application.Interfaces;

/// <summary>
/// Service interface for audit logging operations.
/// Migration note: Replaces custom ActionFilter-based logging in legacy MVC 6.0.
/// In .NET 8, this is injected via DI and called from controllers or middleware.
/// </summary>
public interface IAuditLogService
{
    /// <summary>Records an audit log entry.</summary>
    Task LogAsync(string action, string entityType, string? entityId = null,
        string? userId = null, string? details = null, string? ipAddress = null,
        CancellationToken cancellationToken = default);

    /// <summary>Gets audit logs for a specific user.</summary>
    Task<IReadOnlyList<AuditLogDto>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>Gets audit logs for a specific entity type.</summary>
    Task<IReadOnlyList<AuditLogDto>> GetByEntityTypeAsync(string entityType, CancellationToken cancellationToken = default);
}
