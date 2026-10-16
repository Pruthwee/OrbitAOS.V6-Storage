using Microsoft.Extensions.Logging;
using OrbitAOS.V8.Application.DTOs;
using OrbitAOS.V8.Application.Interfaces;
using OrbitAOS.V8.Domain.Entities;
using OrbitAOS.V8.Domain.Interfaces;

namespace OrbitAOS.V8.Application.Services;

/// <summary>
/// Implementation of IAuditLogService.
/// Migration note: In legacy ASP.NET MVC 6.0, audit logging was typically done via
/// custom ActionFilters or directly in controller actions. In .NET 8 Clean Architecture,
/// this is a dedicated service injected via DI.
/// 
/// Key .NET 8 changes:
/// - Fully async (no synchronous I/O)
/// - Structured logging with ILogger&lt;T&gt;
/// - No System.Web.HttpContext dependency
/// </summary>
public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _repository;
    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(IAuditLogRepository repository, ILogger<AuditLogService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task LogAsync(string action, string entityType, string? entityId = null,
        string? userId = null, string? details = null, string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Recording audit log: Action={Action}, EntityType={EntityType}", action, entityType);

        var entry = new AuditLog
        {
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            UserId = userId,
            Details = details,
            IpAddress = ipAddress,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(entry, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AuditLogDto>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetByUserIdAsync(userId, cancellationToken);
        return entities.Select(MapToDto).ToList().AsReadOnly();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AuditLogDto>> GetByEntityTypeAsync(string entityType, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetByEntityTypeAsync(entityType, cancellationToken);
        return entities.Select(MapToDto).ToList().AsReadOnly();
    }

    // -------------------------------------------------------------------------
    // Private mapping helpers
    // -------------------------------------------------------------------------

    private static AuditLogDto MapToDto(AuditLog entity) => new()
    {
        Id = entity.Id,
        Action = entity.Action,
        EntityType = entity.EntityType,
        EntityId = entity.EntityId,
        UserId = entity.UserId,
        Details = entity.Details,
        IpAddress = entity.IpAddress,
        CreatedAt = entity.CreatedAt
    };
}
