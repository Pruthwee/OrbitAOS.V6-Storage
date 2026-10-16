using OrbitAOS.V8.Domain.Common;

namespace OrbitAOS.V8.Domain.Interfaces;

/// <summary>
/// Generic repository interface for data access abstraction.
/// Migration note: Replaces direct DbContext usage in legacy MVC 6.0 controllers.
/// In .NET 8 Clean Architecture, repositories are defined in Domain and implemented
/// in Infrastructure, injected via DI into Application services.
/// </summary>
/// <typeparam name="T">The entity type, must derive from BaseEntity.</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>Retrieves an entity by its primary key.</summary>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all entities.</summary>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Adds a new entity to the data store.</summary>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing entity in the data store.</summary>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Deletes an entity from the data store.</summary>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Checks whether an entity with the given ID exists.</summary>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
