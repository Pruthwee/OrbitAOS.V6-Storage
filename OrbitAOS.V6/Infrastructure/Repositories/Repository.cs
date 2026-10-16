// ============================================================
// Infrastructure Layer - Generic Repository Implementation
// Implements IRepository<TEntity, TKey> using EF Core 8.
// ============================================================
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using OrbitAOS.V6.Domain.Interfaces;

namespace OrbitAOS.V6.Infrastructure.Repositories
{
    /// <summary>
    /// Generic EF Core repository providing standard CRUD operations.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TKey">The primary key type.</typeparam>
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        /// <summary>
        /// Initializes a new instance of <see cref="Repository{TEntity, TKey}"/>.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TEntity>> GetAllAsync()
            => await _dbSet.AsNoTracking().ToListAsync();

        /// <inheritdoc />
        public async Task<TEntity?> GetByIdAsync(TKey id)
            => await _dbSet.FindAsync(id);

        /// <inheritdoc />
        public async Task AddAsync(TEntity entity)
            => await _dbSet.AddAsync(entity);

        /// <inheritdoc />
        public void Update(TEntity entity)
            => _dbSet.Update(entity);

        /// <inheritdoc />
        public void Remove(TEntity entity)
            => _dbSet.Remove(entity);

        /// <inheritdoc />
        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Convenience repository with an integer primary key.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public class Repository<TEntity> : Repository<TEntity, int>, IRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Repository{TEntity}"/>.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public Repository(ApplicationDbContext context) : base(context) { }
    }
}
