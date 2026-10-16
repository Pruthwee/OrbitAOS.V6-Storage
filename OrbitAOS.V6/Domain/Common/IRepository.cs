// ============================================================
// Domain/Common/IRepository.cs
// Clean Architecture - Domain Layer
// Generic repository interface (Domain defines the contract)
// ============================================================

using System.Linq.Expressions;

namespace OrbitAOS.V6.Domain.Common;

/// <summary>
/// Generic repository interface defining data access contracts.
/// The Domain layer owns this interface; Infrastructure implements it.
/// This is the Repository Pattern for Clean Architecture on .NET 10.
/// </summary>
/// <typeparam name="T">Entity type that extends BaseEntity.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>Retrieves an entity by its primary key.</summary>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all entities (non-deleted).</summary>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Finds entities matching the given predicate.</summary>
    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>Adds a new entity to the repository.</summary>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing entity.</summary>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Removes an entity (soft-delete if supported).</summary>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Checks whether any entity matches the predicate.</summary>
    Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>Returns a queryable for advanced queries.</summary>
    IQueryable<T> Query();
}
