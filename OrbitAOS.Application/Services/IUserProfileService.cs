using OrbitAOS.Application.Common.Models;
using OrbitAOS.Application.DTOs;

namespace OrbitAOS.Application.Services;

/// <summary>
/// Service interface defining operations for managing user profiles.
/// </summary>
public interface IUserProfileService
{
    /// <summary>Retrieves a user profile by its identifier.</summary>
    Task<Result<UserProfileDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a user profile by the associated Identity user ID.</summary>
    Task<Result<UserProfileDto>> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all user profiles.</summary>
    Task<Result<IReadOnlyList<UserProfileDto>>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Creates a new user profile.</summary>
    Task<Result<UserProfileDto>> CreateAsync(UserProfileDto dto, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing user profile.</summary>
    Task<Result> UpdateAsync(UserProfileDto dto, CancellationToken cancellationToken = default);

    /// <summary>Deletes a user profile by its identifier.</summary>
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
