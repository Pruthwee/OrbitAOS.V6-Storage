using Microsoft.Extensions.Logging;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Domain.Interfaces;

namespace OrbitAOS.V6.Application.Services;

/// <summary>
/// Implementation of <see cref="IUserProfileService"/> providing user profile business logic.
/// Orchestrates domain operations and maps between domain entities and DTOs.
/// </summary>
public class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _repository;
    private readonly ILogger<UserProfileService> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="UserProfileService"/>.
    /// </summary>
    /// <param name="repository">The user profile repository.</param>
    /// <param name="logger">The logger instance.</param>
    public UserProfileService(IUserProfileRepository repository, ILogger<UserProfileService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<UserProfileDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving user profile with ID {Id}", id);
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<UserProfileDto?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving user profile for Identity user {IdentityUserId}", identityUserId);
        var entity = await _repository.GetByIdentityUserIdAsync(identityUserId, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<UserProfileDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all user profiles");
        var entities = await _repository.GetAllAsync(cancellationToken);
        return entities.Select(MapToDto).ToList().AsReadOnly();
    }

    /// <inheritdoc />
    public async Task<UserProfileDto> CreateAsync(UserProfileDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating user profile for {Email}", dto.Email);
        var entity = MapToEntity(dto);
        entity.CreatedAt = DateTime.UtcNow;
        var created = await _repository.AddAsync(entity, cancellationToken);
        return MapToDto(created);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(UserProfileDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating user profile with ID {Id}", dto.Id);
        var existing = await _repository.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new InvalidOperationException($"UserProfile with ID {dto.Id} not found.");

        existing.DisplayName = dto.DisplayName;
        existing.Email = dto.Email;
        existing.IsActive = dto.IsActive;
        existing.LastLoginAt = dto.LastLoginAt;
        existing.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(existing, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting user profile with ID {Id}", id);
        var entity = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"UserProfile with ID {id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
    }

    // -------------------------------------------------------------------------
    // Private mapping helpers
    // -------------------------------------------------------------------------

    private static UserProfileDto MapToDto(UserProfile entity) => new()
    {
        Id = entity.Id,
        IdentityUserId = entity.IdentityUserId,
        DisplayName = entity.DisplayName,
        Email = entity.Email,
        IsActive = entity.IsActive,
        LastLoginAt = entity.LastLoginAt,
        CreatedAt = entity.CreatedAt
    };

    private static UserProfile MapToEntity(UserProfileDto dto) => new()
    {
        Id = dto.Id,
        IdentityUserId = dto.IdentityUserId,
        DisplayName = dto.DisplayName,
        Email = dto.Email,
        IsActive = dto.IsActive,
        LastLoginAt = dto.LastLoginAt
    };
}
