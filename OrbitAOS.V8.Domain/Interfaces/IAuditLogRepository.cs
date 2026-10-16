using OrbitAOS.V8.Domain.Entities;

namespace OrbitAOS.V8.Domain.Interfaces;

/// <summary>
/// Repository interface for AuditLog entities.
/// Migration note: Replaces custom logging implementations in legacy MVC 6.0.
/// </summary>
public interface IAuditLogRepository : IRepository<AuditLog>
{
    /// <summary>Retrieves audit logs for a specific user.</summary>
    Task<IReadOnlyList<AuditLog>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves audit logs for a specific entity type.</summary>
    Task<IReadOnlyList<AuditLog>> GetByEntityTypeAsync(string entityType, CancellationToken cancellationToken = default);
}
