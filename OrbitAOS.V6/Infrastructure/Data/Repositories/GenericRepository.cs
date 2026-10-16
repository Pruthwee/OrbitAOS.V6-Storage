// ============================================================
// Infrastructure/Data/Repositories/GenericRepository.cs
// Clean Architecture - Infrastructure Layer
// Generic EF Core 10 repository implementation
// Implements Domain/Common/IRepository<T>
// ============================================================

using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Domain.Common;
using System.Linq.Expressions;

namespace OrbitAOS.V6.Infrastructure.Data.Repositories;

/// <summary>
/// Generic EF Core 10 repository implementation.
/// Provides standard CRUD operations for all domain entities.
/// 
/// Migration note: Replaces legacy data access patterns:
/// - Direct DbContext usage in controllers
/// - ADO.NET SqlCommand/SqlDataReader patterns
/// - EF6 ObjectContext/DbContext patterns
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public class GenericRepository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<T>();
    }

    /// <inheritdoc/>
    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <inheritdoc/>
    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual IQueryable<T> Query()
    {
        return _dbSet.AsQueryable();
    }
}
