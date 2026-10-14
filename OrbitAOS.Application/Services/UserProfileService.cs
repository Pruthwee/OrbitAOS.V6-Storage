using Microsoft.Extensions.Logging;
using OrbitAOS.Application.DTOs;
using OrbitAOS.Application.Interfaces;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;

namespace OrbitAOS.Application.Services;

/// <summary>
/// Application service for user profile management.
/// Implements business logic for user profile use cases.
/// </summary>
public class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ILogger<UserProfileService> _logger;

    public UserProfileService(
        IUserProfileRepository userProfileRepository,
        ILogger<UserProfileService> logger)
    {
        _userProfileRepository = userProfileRepository;
        _logger = logger;
    }

    public async Task<UserProfileDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving user profile with ID {Id}", id);
        var profile = await _userProfileRepository.GetByIdAsync(id, cancellationToken);
        return profile is null ? null : MapToDto(profile);
    }

    public async Task<UserProfileDto?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving user profile for identity user {IdentityUserId}", identityUserId);
        var profile = await _userProfileRepository.GetByIdentityUserIdAsync(identityUserId, cancellationToken);
        return profile is null ? null : MapToDto(profile);
    }

    public async Task<IReadOnlyList<UserProfileDto>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all active user profiles");
        var profiles = await _userProfileRepository.GetActiveUsersAsync(cancellationToken);
        return profiles.Select(MapToDto).ToList().AsReadOnly();
    }

    public async Task<UserProfileDto> CreateAsync(CreateUserProfileRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating user profile for identity user {IdentityUserId}", request.IdentityUserId);

        var profile = new UserProfile
        {
            IdentityUserId = request.IdentityUserId,
            DisplayName = request.DisplayName,
            Department = request.Department,
            Position = request.Position,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _userProfileRepository.AddAsync(profile, cancellationToken);
        return MapToDto(created);
    }

    public async Task UpdateAsync(int id, UpdateUserProfileRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating user profile with ID {Id}", id);

        var profile = await _userProfileRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"User profile with ID {id} not found.");

        profile.DisplayName = request.DisplayName;
        profile.Department = request.Department;
        profile.Position = request.Position;
        profile.UpdatedAt = DateTime.UtcNow;

        await _userProfileRepository.UpdateAsync(profile, cancellationToken);
    }

    public async Task DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deactivating user profile with ID {Id}", id);

        var profile = await _userProfileRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"User profile with ID {id} not found.");

        profile.IsActive = false;
        profile.UpdatedAt = DateTime.UtcNow;

        await _userProfileRepository.UpdateAsync(profile, cancellationToken);
    }

    private static UserProfileDto MapToDto(UserProfile profile) => new()
    {
        Id = profile.Id,
        IdentityUserId = profile.IdentityUserId,
        DisplayName = profile.DisplayName,
        Department = profile.Department,
        Position = profile.Position,
        IsActive = profile.IsActive,
        LastLoginAt = profile.LastLoginAt,
        CreatedAt = profile.CreatedAt
    };
}
