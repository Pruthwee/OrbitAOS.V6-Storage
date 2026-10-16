using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;
using OrbitAOS.Infrastructure.Data;

namespace OrbitAOS.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for UserProfile entity with domain-specific query methods.
/// </summary>
public class UserProfileRepository : Repository<UserProfile>, IUserProfileRepository
{
    /// <summary>Initializes a new instance of UserProfileRepository.</summary>
    public UserProfileRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<UserProfile?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}
