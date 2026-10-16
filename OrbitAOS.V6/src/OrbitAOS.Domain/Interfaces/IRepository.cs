using OrbitAOS.Domain.Common;

namespace OrbitAOS.Domain.Interfaces;

/// <summary>
/// Generic repository interface defining standard CRUD operations for domain entities.
/// </summary>
/// <typeparam name="T">The entity type that inherits from <see cref="BaseEntity"/>.</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>Retrieves an entity by its unique identifier asynchronously.</summary>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all entities asynchronously.</summary>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Adds a new entity asynchronously.</summary>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing entity asynchronously.</summary>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Deletes an entity by its unique identifier asynchronously.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Checks whether an entity with the given identifier exists.</summary>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
