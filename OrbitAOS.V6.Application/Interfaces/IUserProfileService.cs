using OrbitAOS.V6.Application.DTOs;

namespace OrbitAOS.V6.Application.Interfaces;

/// <summary>
/// Service interface for user profile business operations.
/// Defines the application-level contract for managing user profiles.
/// </summary>
public interface IUserProfileService
{
    /// <summary>Retrieves a user profile by its identifier.</summary>
    /// <param name="id">The profile identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user profile DTO if found; otherwise null.</returns>
    Task<UserProfileDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a user profile by the associated Identity user ID.</summary>
    /// <param name="identityUserId">The Identity user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user profile DTO if found; otherwise null.</returns>
    Task<UserProfileDto?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all user profiles.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only list of all user profile DTOs.</returns>
    Task<IReadOnlyList<UserProfileDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Creates a new user profile.</summary>
    /// <param name="dto">The user profile data to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created user profile DTO with generated ID.</returns>
    Task<UserProfileDto> CreateAsync(UserProfileDto dto, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing user profile.</summary>
    /// <param name="dto">The user profile data with updated values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAsync(UserProfileDto dto, CancellationToken cancellationToken = default);

    /// <summary>Deletes a user profile by its identifier.</summary>
    /// <param name="id">The profile identifier to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
