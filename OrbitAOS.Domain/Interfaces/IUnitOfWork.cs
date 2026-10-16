namespace OrbitAOS.Domain.Interfaces;

/// <summary>
/// Unit of Work interface for coordinating multiple repository operations
/// within a single database transaction.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>Gets the UserProfile repository.</summary>
    IUserProfileRepository UserProfiles { get; }

    /// <summary>Commits all pending changes to the database.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
