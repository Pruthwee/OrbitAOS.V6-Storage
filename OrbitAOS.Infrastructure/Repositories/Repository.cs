using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Common;
using OrbitAOS.Domain.Interfaces;
using OrbitAOS.Infrastructure.Data;

namespace OrbitAOS.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation using Entity Framework Core for data access.
/// </summary>
/// <typeparam name="T">The entity type that inherits from BaseEntity.</typeparam>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    /// <summary>The database context used for data access.</summary>
    protected readonly ApplicationDbContext _context;

    /// <summary>Initializes a new instance of Repository.</summary>
    public Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<T>().AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <inheritdoc />
    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public virtual async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().AnyAsync(e => e.Id == id, cancellationToken);
    }
}
