using Microsoft.EntityFrameworkCore;
using OrbitAOS.V8.Domain.Entities;
using OrbitAOS.V8.Domain.Interfaces;
using OrbitAOS.V8.Infrastructure.Data;

namespace OrbitAOS.V8.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for AuditLog entities.
/// Migration note: Replaces custom logging implementations in legacy MVC 6.0.
/// </summary>
public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AuditLog>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AuditLog>> GetByEntityTypeAsync(string entityType, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.EntityType == entityType)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
