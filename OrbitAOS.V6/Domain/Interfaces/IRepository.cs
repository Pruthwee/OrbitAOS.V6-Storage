// ============================================================
// Domain Layer - Generic Repository Interface
// Defines the contract for all data-access repositories.
// ============================================================
namespace OrbitAOS.V6.Domain.Interfaces
{
    /// <summary>
    /// Generic repository interface for CRUD operations.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TKey">The primary key type.</typeparam>
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        /// <summary>Retrieves all entities asynchronously.</summary>
        Task<IEnumerable<TEntity>> GetAllAsync();

        /// <summary>Retrieves an entity by its primary key.</summary>
        Task<TEntity?> GetByIdAsync(TKey id);

        /// <summary>Adds a new entity.</summary>
        Task AddAsync(TEntity entity);

        /// <summary>Updates an existing entity.</summary>
        void Update(TEntity entity);

        /// <summary>Removes an entity.</summary>
        void Remove(TEntity entity);

        /// <summary>Persists all pending changes to the database.</summary>
        Task<int> SaveChangesAsync();
    }

    /// <summary>
    /// Convenience repository interface with an integer primary key.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : class { }
}
