using Microsoft.EntityFrameworkCore;
using OrbitAOS.V8.Domain.Common;
using OrbitAOS.V8.Domain.Interfaces;
using OrbitAOS.V8.Infrastructure.Data;

namespace OrbitAOS.V8.Infrastructure.Repositories;

/// <summary>
/// Generic EF Core repository implementation.
/// 
/// Migration notes from legacy ASP.NET MVC 6.0:
/// - Replaces direct DbContext usage in controllers (anti-pattern in legacy MVC)
/// - Uses EF Core 8.0 async methods (SaveChangesAsync, FindAsync, etc.)
/// - No more ObjectSet&lt;T&gt; — uses DbSet&lt;T&gt;
/// - No more Database.ExecuteSqlCommand() — uses ExecuteSqlRaw()
/// - Async/await throughout (no .Result or .Wait() blocking calls)
/// 
/// Breaking changes from EF6:
/// - DbContext.Entry(entity).State = EntityState.Modified → use Update(entity)
/// - No lazy loading by default — use Include() for related data
/// - No Database.SetInitializer() — use EF Core Migrations
/// </summary>
/// <typeparam name="T">Entity type deriving from BaseEntity.</typeparam>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <inheritdoc />
    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(e => e.Id == id, cancellationToken);
    }
}
