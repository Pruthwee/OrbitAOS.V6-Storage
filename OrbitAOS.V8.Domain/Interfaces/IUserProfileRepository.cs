using OrbitAOS.V8.Domain.Entities;

namespace OrbitAOS.V8.Domain.Interfaces;

/// <summary>
/// Repository interface for UserProfile entities.
/// Migration note: Replaces direct ApplicationDbContext queries in legacy MVC controllers.
/// </summary>
public interface IUserProfileRepository : IRepository<UserProfile>
{
    /// <summary>Finds a user profile by the associated Identity user ID.</summary>
    Task<UserProfile?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);

    /// <summary>Finds a user profile by email address.</summary>
    Task<UserProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
