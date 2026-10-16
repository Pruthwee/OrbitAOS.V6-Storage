using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V8.Domain.Entities;
using OrbitAOS.V8.Infrastructure.Data;
using OrbitAOS.V8.Infrastructure.Repositories;
using Xunit;

namespace OrbitAOS.V8.Tests.Infrastructure;

/// <summary>
/// Integration tests for UserProfileRepository using EF Core InMemory provider.
/// 
/// Migration notes from legacy ASP.NET MVC 6.0 testing:
/// - Uses EF Core InMemory provider instead of SQL Server for tests
/// - No more Effort (EF6 in-memory provider) — use EF Core InMemory
/// - Async test methods throughout
/// - Each test gets a fresh DbContext to avoid state pollution
/// </summary>
public class UserProfileRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserProfileRepository _repository;

    public UserProfileRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new UserProfileRepository(_context);
    }

    [Fact]
    public async Task AddAsync_PersistsEntity()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "identity-1",
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddAsync(profile);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        var saved = await _context.UserProfiles.FindAsync(result.Id);
        saved.Should().NotBeNull();
        saved!.DisplayName.Should().Be("Test User");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenExists()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "identity-2",
            DisplayName = "Find Me",
            Email = "findme@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(profile.Id);

        // Assert
        result.Should().NotBeNull();
        result!.DisplayName.Should().Be("Find Me");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(9999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdentityUserIdAsync_ReturnsCorrectProfile()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "identity-specific",
            DisplayName = "Specific User",
            Email = "specific@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdentityUserIdAsync("identity-specific");

        // Assert
        result.Should().NotBeNull();
        result!.DisplayName.Should().Be("Specific User");
    }

    [Fact]
    public async Task GetByIdentityUserIdAsync_ReturnsNull_WhenNotFound()
    {
        // Act
        var result = await _repository.GetByIdentityUserIdAsync("nonexistent-id");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProfiles()
    {
        // Arrange
        _context.UserProfiles.AddRange(
            new UserProfile { IdentityUserId = "id-1", DisplayName = "User 1", Email = "u1@test.com", IsActive = true, CreatedAt = DateTime.UtcNow },
            new UserProfile { IdentityUserId = "id-2", DisplayName = "User 2", Email = "u2@test.com", IsActive = true, CreatedAt = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_PersistsChanges()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "identity-update",
            DisplayName = "Before Update",
            Email = "before@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();

        // Act
        profile.DisplayName = "After Update";
        await _repository.UpdateAsync(profile);

        // Assert
        var updated = await _context.UserProfiles.FindAsync(profile.Id);
        updated!.DisplayName.Should().Be("After Update");
    }

    [Fact]
    public async Task DeleteAsync_RemovesEntity()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "identity-delete",
            DisplayName = "To Delete",
            Email = "delete@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(profile);

        // Assert
        var deleted = await _context.UserProfiles.FindAsync(profile.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenEntityExists()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "identity-exists",
            DisplayName = "Exists",
            Email = "exists@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(profile.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenEntityNotExists()
    {
        // Act
        var result = await _repository.ExistsAsync(9999);

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
