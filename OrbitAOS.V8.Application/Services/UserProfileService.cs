using Microsoft.Extensions.Logging;
using OrbitAOS.V8.Application.DTOs;
using OrbitAOS.V8.Application.Interfaces;
using OrbitAOS.V8.Domain.Entities;
using OrbitAOS.V8.Domain.Interfaces;

namespace OrbitAOS.V8.Application.Services;

/// <summary>
/// Implementation of IUserProfileService.
/// Migration note: In legacy ASP.NET MVC 6.0, user profile logic was scattered across
/// AccountController and HomeController with direct DbContext access. This service
/// centralizes that logic and uses the repository pattern for data access.
/// 
/// Key .NET 8 changes:
/// - Async/await throughout (no synchronous blocking calls)
/// - Constructor injection via DI (no DependencyResolver.Current)
/// - ILogger&lt;T&gt; instead of log4net or custom logging
/// - Nullable reference types enabled
/// </summary>
public class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _repository;
    private readonly ILogger<UserProfileService> _logger;

    public UserProfileService(IUserProfileRepository repository, ILogger<UserProfileService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<UserProfileDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving user profile with ID {Id}", id);
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<UserProfileDto?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving user profile for Identity user {IdentityUserId}", identityUserId);
        var entity = await _repository.GetByIdentityUserIdAsync(identityUserId, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<UserProfileDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving all user profiles");
        var entities = await _repository.GetAllAsync(cancellationToken);
        return entities.Select(MapToDto).ToList().AsReadOnly();
    }

    /// <inheritdoc />
    public async Task<UserProfileDto> CreateAsync(UserProfileDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating user profile for Identity user {IdentityUserId}", dto.IdentityUserId);

        var entity = new UserProfile
        {
            IdentityUserId = dto.IdentityUserId,
            DisplayName = dto.DisplayName,
            Email = dto.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(entity, cancellationToken);
        return MapToDto(created);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(UserProfileDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating user profile with ID {Id}", dto.Id);

        var entity = await _repository.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new InvalidOperationException($"UserProfile with ID {dto.Id} not found.");

        entity.DisplayName = dto.DisplayName;
        entity.Email = dto.Email;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deactivating user profile with ID {Id}", id);

        var entity = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"UserProfile with ID {id} not found.");

        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(entity, cancellationToken);
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
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };
}
