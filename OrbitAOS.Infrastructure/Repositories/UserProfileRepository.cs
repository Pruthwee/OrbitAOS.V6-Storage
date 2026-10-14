using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;
using OrbitAOS.Infrastructure.Data;

namespace OrbitAOS.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for UserProfile domain entity.
/// Provides user-profile-specific data access operations.
/// </summary>
public class UserProfileRepository : Repository<UserProfile>, IUserProfileRepository
{
    public UserProfileRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<UserProfile?> GetByIdentityUserIdAsync(
        string identityUserId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId, cancellationToken);
    }

    public async Task<IReadOnlyList<UserProfile>> GetActiveUsersAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(u => u.IsActive)
            .OrderBy(u => u.DisplayName)
            .ToListAsync(cancellationToken);
    }
}
