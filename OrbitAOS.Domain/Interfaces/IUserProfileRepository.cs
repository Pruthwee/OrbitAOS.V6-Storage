using OrbitAOS.Domain.Entities;

namespace OrbitAOS.Domain.Interfaces;

/// <summary>
/// Repository interface for UserProfile domain entity.
/// Extends the generic repository with user-profile-specific queries.
/// </summary>
public interface IUserProfileRepository : IRepository<UserProfile>
{
    Task<UserProfile?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserProfile>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
}
