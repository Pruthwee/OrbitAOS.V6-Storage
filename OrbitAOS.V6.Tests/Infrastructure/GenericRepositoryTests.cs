// ============================================================
// Tests/Infrastructure/GenericRepositoryTests.cs
// Unit Tests - EF Core InMemory for .NET 10
// Tests for GenericRepository using InMemory database
// ============================================================

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Infrastructure.Data;
using OrbitAOS.V6.Infrastructure.Data.Repositories;
using Xunit;

namespace OrbitAOS.V6.Tests.Infrastructure;

/// <summary>
/// Unit tests for GenericRepository using EF Core InMemory provider.
/// Demonstrates testing the Infrastructure layer in isolation.
///
/// Migration note: EF Core InMemory replaces legacy in-memory testing approaches
/// that required mocking ObjectContext or using SQLite in-memory.
/// </summary>
public class GenericRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly GenericRepository<BaseEntityTestImpl> _repository;

    public GenericRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new GenericRepository<BaseEntityTestImpl>(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntity_AndReturnWithId()
    {
        // Arrange
        var entity = new BaseEntityTestImpl { Name = "Test Entity" };

        // Act
        var result = await _repository.AddAsync(entity);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("Test Entity");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenExists()
    {
        // Arrange
        var entity = new BaseEntityTestImpl { Name = "Find Me" };
        await _repository.AddAsync(entity);

        // Act
        var result = await _repository.GetByIdAsync(entity.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Find Me");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(99999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        await _repository.AddAsync(new BaseEntityTestImpl { Name = "Entity 1" });
        await _repository.AddAsync(new BaseEntityTestImpl { Name = "Entity 2" });
        await _repository.AddAsync(new BaseEntityTestImpl { Name = "Entity 3" });

        // Act
        var results = await _repository.GetAllAsync();

        // Assert
        results.Should().HaveCount(3);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnMatchingEntities()
    {
        // Arrange
        await _repository.AddAsync(new BaseEntityTestImpl { Name = "Alpha" });
        await _repository.AddAsync(new BaseEntityTestImpl { Name = "Beta" });
        await _repository.AddAsync(new BaseEntityTestImpl { Name = "Alpha Two" });

        // Act
        var results = await _repository.FindAsync(e => e.Name.StartsWith("Alpha"));

        // Assert
        results.Should().HaveCount(2);
        results.All(e => e.Name.StartsWith("Alpha")).Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenEntityExists()
    {
        // Arrange
        await _repository.AddAsync(new BaseEntityTestImpl { Name = "Exists" });

        // Act
        var exists = await _repository.ExistsAsync(e => e.Name == "Exists");

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenEntityDoesNotExist()
    {
        // Act
        var exists = await _repository.ExistsAsync(e => e.Name == "Does Not Exist");

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        // Arrange
        var entity = await _repository.AddAsync(new BaseEntityTestImpl { Name = "To Delete" });

        // Act
        await _repository.DeleteAsync(entity);
        var result = await _repository.GetByIdAsync(entity.Id);

        // Assert
        result.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

/// <summary>
/// Concrete test implementation of BaseEntity for repository tests.
/// BaseEntity is abstract, so we need a concrete subclass for testing.
/// </summary>
public class BaseEntityTestImpl : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}
