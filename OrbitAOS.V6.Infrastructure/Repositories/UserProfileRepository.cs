using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Domain.Interfaces;
using OrbitAOS.V6.Infrastructure.Data;

namespace OrbitAOS.V6.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IUserProfileRepository"/>.
/// Provides user-profile-specific query operations on top of the generic repository.
/// </summary>
public class UserProfileRepository : Repository<UserProfile>, IUserProfileRepository
{
    /// <summary>
    /// Initializes a new instance of <see cref="UserProfileRepository"/>.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public UserProfileRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<UserProfile?> GetByIdentityUserIdAsync(
        string identityUserId,
        CancellationToken cancellationToken = default)
        => await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId, cancellationToken);

    /// <inheritdoc />
    public async Task<UserProfile?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
        => await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
}
