using OrbitAOS.Application.DTOs;
using OrbitAOS.Application.Interfaces;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;

namespace OrbitAOS.Application.Services;

/// <summary>
/// Service implementation for user profile business operations.
/// Orchestrates domain entities and repository interactions.
/// </summary>
public class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _userProfileRepository;

    /// <summary>
    /// Initializes a new instance of <see cref="UserProfileService"/>.
    /// </summary>
    /// <param name="userProfileRepository">The user profile repository.</param>
    public UserProfileService(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    /// <inheritdoc/>
    public async Task<UserProfileDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _userProfileRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    /// <inheritdoc/>
    public async Task<UserProfileDto?> GetByIdentityUserIdAsync(
        string identityUserId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _userProfileRepository.GetByIdentityUserIdAsync(identityUserId, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UserProfileDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _userProfileRepository.GetAllAsync(cancellationToken);
        return entities.Select(MapToDto);
    }

    /// <inheritdoc/>
    public async Task<UserProfileDto> CreateAsync(UserProfileDto dto, CancellationToken cancellationToken = default)
    {
        var entity = MapToEntity(dto);
        var created = await _userProfileRepository.AddAsync(entity, cancellationToken);
        return MapToDto(created);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(UserProfileDto dto, CancellationToken cancellationToken = default)
    {
        var entity = MapToEntity(dto);
        await _userProfileRepository.UpdateAsync(entity, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _userProfileRepository.DeleteAsync(id, cancellationToken);
    }

    // ─── Private Mapping Helpers ────────────────────────────────────────────────

    private static UserProfileDto MapToDto(UserProfile entity) => new()
    {
        Id = entity.Id,
        IdentityUserId = entity.IdentityUserId,
        DisplayName = entity.DisplayName,
        Email = entity.Email,
        IsActive = entity.IsActive,
        CreatedAt = entity.CreatedAt
    };

    private static UserProfile MapToEntity(UserProfileDto dto) => new()
    {
        Id = dto.Id,
        IdentityUserId = dto.IdentityUserId,
        DisplayName = dto.DisplayName,
        Email = dto.Email,
        IsActive = dto.IsActive,
        CreatedAt = dto.CreatedAt
    };
}
