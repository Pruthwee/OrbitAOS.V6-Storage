using OrbitAOS.V6.Domain.Entities;

namespace OrbitAOS.V6.Domain.Interfaces;

/// <summary>
/// Repository interface for <see cref="UserProfile"/> domain entity.
/// Extends the generic repository with user-profile-specific query operations.
/// </summary>
public interface IUserProfileRepository : IRepository<UserProfile>
{
    /// <summary>Retrieves a user profile by the associated ASP.NET Core Identity user ID.</summary>
    /// <param name="identityUserId">The Identity user ID string.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user profile if found; otherwise null.</returns>
    Task<UserProfile?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a user profile by email address.</summary>
    /// <param name="email">The email address to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user profile if found; otherwise null.</returns>
    Task<UserProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
