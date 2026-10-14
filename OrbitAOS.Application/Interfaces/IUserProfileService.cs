using OrbitAOS.Application.DTOs;

namespace OrbitAOS.Application.Interfaces;

/// <summary>
/// Service interface for user profile management operations.
/// Defines the application-level contract for user profile use cases.
/// </summary>
public interface IUserProfileService
{
    Task<UserProfileDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserProfileDto?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserProfileDto>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<UserProfileDto> CreateAsync(CreateUserProfileRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateUserProfileRequest request, CancellationToken cancellationToken = default);
    Task DeactivateAsync(int id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Request model for creating a new user profile.
/// </summary>
public record CreateUserProfileRequest(
    string IdentityUserId,
    string DisplayName,
    string? Department,
    string? Position);

/// <summary>
/// Request model for updating an existing user profile.
/// </summary>
public record UpdateUserProfileRequest(
    string DisplayName,
    string? Department,
    string? Position);
