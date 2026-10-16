using OrbitAOS.Application.DTOs;
using OrbitAOS.Application.Interfaces;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;

namespace OrbitAOS.Application.Services;

/// <summary>
/// Application service implementing UserProfile business logic.
/// Orchestrates domain operations via the Unit of Work pattern.
/// </summary>
public class UserProfileService : IUserProfileService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of <see cref="UserProfileService"/>.</summary>
    public UserProfileService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<UserProfileDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.UserProfiles.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<UserProfileDto?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.UserProfiles.GetByIdentityUserIdAsync(identityUserId, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<UserProfileDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.UserProfiles.GetAllAsync(cancellationToken);
        return entities.Select(MapToDto).ToList().AsReadOnly();
    }

    /// <inheritdoc />
    public async Task<UserProfileDto> CreateAsync(UserProfileDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new UserProfile
        {
            IdentityUserId = dto.IdentityUserId,
            DisplayName = dto.DisplayName,
            Email = dto.Email,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _unitOfWork.UserProfiles.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(created);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(UserProfileDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.UserProfiles.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new InvalidOperationException($"UserProfile with ID {dto.Id} was not found.");

        entity.DisplayName = dto.DisplayName;
        entity.Email = dto.Email;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.UserProfiles.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.UserProfiles.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private static UserProfileDto MapToDto(UserProfile entity) => new()
    {
        Id = entity.Id,
        IdentityUserId = entity.IdentityUserId,
        DisplayName = entity.DisplayName,
        Email = entity.Email,
        IsActive = entity.IsActive,
        CreatedAt = entity.CreatedAt
    };
}
