using OrbitAOS.Application.DTOs;

namespace OrbitAOS.Application.Interfaces;

/// <summary>
/// Service interface for UserProfile application-level operations.
/// Encapsulates business logic and orchestrates domain operations.
/// </summary>
public interface IUserProfileService
{
    /// <summary>Gets a user profile by its primary key.</summary>
    Task<UserProfileDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Gets a user profile by the ASP.NET Core Identity user ID.</summary>
    Task<UserProfileDto?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);

    /// <summary>Gets all user profiles.</summary>
    Task<IReadOnlyList<UserProfileDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Creates a new user profile.</summary>
    Task<UserProfileDto> CreateAsync(UserProfileDto dto, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing user profile.</summary>
    Task UpdateAsync(UserProfileDto dto, CancellationToken cancellationToken = default);

    /// <summary>Deletes a user profile by its primary key.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
