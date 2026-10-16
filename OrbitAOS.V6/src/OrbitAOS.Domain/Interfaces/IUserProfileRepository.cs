using OrbitAOS.Domain.Entities;

namespace OrbitAOS.Domain.Interfaces;

/// <summary>
/// Repository interface for <see cref="UserProfile"/> entity operations.
/// </summary>
public interface IUserProfileRepository : IRepository<UserProfile>
{
    /// <summary>Retrieves a user profile by the associated ASP.NET Core Identity user ID.</summary>
    Task<UserProfile?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a user profile by email address.</summary>
    Task<UserProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
