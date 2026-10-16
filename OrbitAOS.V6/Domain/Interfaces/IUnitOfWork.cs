// ============================================================
// Domain Layer - Unit of Work Interface
// Coordinates multiple repositories in a single transaction.
// ============================================================
namespace OrbitAOS.V6.Domain.Interfaces
{
    /// <summary>
    /// Unit of Work pattern interface.
    /// Provides a single save point across multiple repositories.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>Commits all pending changes to the database.</summary>
        Task<int> CommitAsync();

        /// <summary>Rolls back any pending changes.</summary>
        Task RollbackAsync();
    }
}
