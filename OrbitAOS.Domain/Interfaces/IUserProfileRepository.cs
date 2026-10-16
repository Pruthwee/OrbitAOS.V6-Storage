using OrbitAOS.Domain.Entities;

namespace OrbitAOS.Domain.Interfaces;

/// <summary>
/// Repository interface for UserProfile domain entity.
/// Follows the Repository pattern for data access abstraction.
/// </summary>
public interface IUserProfileRepository
{
    /// <summary>Retrieves a user profile by its primary key.</summary>
    Task<UserProfile?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a user profile by the ASP.NET Core Identity user ID.</summary>
    Task<UserProfile?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all user profiles.</summary>
    Task<IReadOnlyList<UserProfile>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Adds a new user profile to the repository.</summary>
    Task<UserProfile> AddAsync(UserProfile userProfile, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing user profile.</summary>
    Task UpdateAsync(UserProfile userProfile, CancellationToken cancellationToken = default);

    /// <summary>Deletes a user profile by its primary key.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
