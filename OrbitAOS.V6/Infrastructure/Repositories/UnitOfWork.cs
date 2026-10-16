// ============================================================
// Infrastructure Layer - Unit of Work Implementation
// Wraps ApplicationDbContext to coordinate multiple repositories.
// ============================================================
using OrbitAOS.V6.Data;
using OrbitAOS.V6.Domain.Interfaces;

namespace OrbitAOS.V6.Infrastructure.Repositories
{
    /// <summary>
    /// Unit of Work implementation using EF Core 8.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of <see cref="UnitOfWork"/>.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<int> CommitAsync()
            => await _context.SaveChangesAsync();

        /// <inheritdoc />
        public async Task RollbackAsync()
        {
            // Detach all tracked entities to discard pending changes.
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Releases managed resources.</summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}
