using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;
using OrbitAOS.Infrastructure.Data;

namespace OrbitAOS.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IUserProfileRepository"/>.
/// </summary>
public class UserProfileRepository : IUserProfileRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>Initializes a new instance of <see cref="UserProfileRepository"/>.</summary>
    public UserProfileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<UserProfile?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.UserProfiles.FindAsync(new object[] { id }, cancellationToken);

    /// <inheritdoc />
    public async Task<UserProfile?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default)
        => await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<UserProfile>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.UserProfiles.ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<UserProfile> AddAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
    {
        await _context.UserProfiles.AddAsync(userProfile, cancellationToken);
        return userProfile;
    }

    /// <inheritdoc />
    public Task UpdateAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
    {
        _context.UserProfiles.Update(userProfile);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity is not null)
            _context.UserProfiles.Remove(entity);
    }
}
