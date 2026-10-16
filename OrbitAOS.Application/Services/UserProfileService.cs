using OrbitAOS.Application.Common.Models;
using OrbitAOS.Application.DTOs;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;

namespace OrbitAOS.Application.Services;

/// <summary>
/// Implementation of IUserProfileService providing business logic for user profile management.
/// </summary>
public class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _userProfileRepository;

    /// <summary>Initializes a new instance of UserProfileService.</summary>
    public UserProfileService(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    /// <inheritdoc />
    public async Task<Result<UserProfileDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var profile = await _userProfileRepository.GetByIdAsync(id, cancellationToken);
        if (profile == null)
            return Result<UserProfileDto>.Failure($"User profile with ID {id} was not found.");

        return Result<UserProfileDto>.Success(MapToDto(profile));
    }

    /// <inheritdoc />
    public async Task<Result<UserProfileDto>> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(identityUserId))
            return Result<UserProfileDto>.Failure("Identity user ID cannot be empty.");

        var profile = await _userProfileRepository.GetByIdentityUserIdAsync(identityUserId, cancellationToken);
        if (profile == null)
            return Result<UserProfileDto>.Failure($"User profile for identity user '{identityUserId}' was not found.");

        return Result<UserProfileDto>.Success(MapToDto(profile));
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<UserProfileDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var profiles = await _userProfileRepository.GetAllAsync(cancellationToken);
        var dtos = profiles.Select(MapToDto).ToList().AsReadOnly();
        return Result<IReadOnlyList<UserProfileDto>>.Success(dtos);
    }

    /// <inheritdoc />
    public async Task<Result<UserProfileDto>> CreateAsync(UserProfileDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            return Result<UserProfileDto>.Failure("User profile data cannot be null.");

        if (string.IsNullOrWhiteSpace(dto.Email))
            return Result<UserProfileDto>.Failure("Email address is required.");

        var entity = new UserProfile
        {
            IdentityUserId = dto.IdentityUserId,
            DisplayName = dto.DisplayName,
            Email = dto.Email,
            IsActive = dto.IsActive,
            LastLoginAt = dto.LastLoginAt,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _userProfileRepository.AddAsync(entity, cancellationToken);
        return Result<UserProfileDto>.Success(MapToDto(created));
    }

    /// <inheritdoc />
    public async Task<Result> UpdateAsync(UserProfileDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            return Result.Failure("User profile data cannot be null.");

        var existing = await _userProfileRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (existing == null)
            return Result.Failure($"User profile with ID {dto.Id} was not found.");

        existing.DisplayName = dto.DisplayName;
        existing.Email = dto.Email;
        existing.IsActive = dto.IsActive;
        existing.LastLoginAt = dto.LastLoginAt;
        existing.UpdatedAt = DateTime.UtcNow;

        await _userProfileRepository.UpdateAsync(existing, cancellationToken);
        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var exists = await _userProfileRepository.ExistsAsync(id, cancellationToken);
        if (!exists)
            return Result.Failure($"User profile with ID {id} was not found.");

        await _userProfileRepository.DeleteAsync(id, cancellationToken);
        return Result.Success();
    }

    /// <summary>Maps a UserProfile domain entity to a UserProfileDto.</summary>
    private static UserProfileDto MapToDto(UserProfile profile) => new()
    {
        Id = profile.Id,
        IdentityUserId = profile.IdentityUserId,
        DisplayName = profile.DisplayName,
        Email = profile.Email,
        IsActive = profile.IsActive,
        LastLoginAt = profile.LastLoginAt,
        CreatedAt = profile.CreatedAt
    };
}
