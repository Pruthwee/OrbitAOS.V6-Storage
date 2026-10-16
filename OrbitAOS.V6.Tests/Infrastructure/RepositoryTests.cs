// ============================================================
// Unit Tests - Repository Tests
// Tests the generic Repository using EF Core InMemory provider.
// ============================================================
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Infrastructure.Repositories;
using Xunit;

namespace OrbitAOS.V6.Tests.Infrastructure
{
    // Sample entity for testing the generic repository
    public class SampleEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Unit tests for the generic <see cref="Repository{TEntity}"/>.
    /// Uses EF Core InMemory provider to avoid a real database.
    /// </summary>
    public class RepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Repository<SampleEntity> _repository;

        /// <summary>
        /// Initializes an in-memory database context for each test.
        /// </summary>
        public RepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new Repository<SampleEntity>(_context);
        }

        /// <summary>
        /// GetAllAsync should return all seeded entities.
        /// </summary>
        [Fact]
        public async Task GetAllAsync_ReturnsAllEntities()
        {
            // Arrange
            _context.Set<SampleEntity>().AddRange(
                new SampleEntity { Name = "Alpha" },
                new SampleEntity { Name = "Beta" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
        }

        /// <summary>
        /// AddAsync followed by SaveChangesAsync should persist the entity.
        /// </summary>
        [Fact]
        public async Task AddAsync_PersistsEntity()
        {
            // Arrange
            var entity = new SampleEntity { Name = "Gamma" };

            // Act
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            // Assert
            var all = await _repository.GetAllAsync();
            all.Should().ContainSingle(e => e.Name == "Gamma");
        }

        /// <summary>
        /// Remove should delete the entity from the database.
        /// </summary>
        [Fact]
        public async Task Remove_DeletesEntity()
        {
            // Arrange
            var entity = new SampleEntity { Name = "Delta" };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            // Act
            _repository.Remove(entity);
            await _repository.SaveChangesAsync();

            // Assert
            var all = await _repository.GetAllAsync();
            all.Should().BeEmpty();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
