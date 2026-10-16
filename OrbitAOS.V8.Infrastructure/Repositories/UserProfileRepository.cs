using Microsoft.EntityFrameworkCore;
using OrbitAOS.V8.Domain.Entities;
using OrbitAOS.V8.Domain.Interfaces;
using OrbitAOS.V8.Infrastructure.Data;

namespace OrbitAOS.V8.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for UserProfile entities.
/// Migration note: Replaces direct DbContext queries in legacy MVC 6.0 AccountController.
/// </summary>
public class UserProfileRepository : Repository<UserProfile>, IUserProfileRepository
{
    public UserProfileRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<UserProfile?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}
