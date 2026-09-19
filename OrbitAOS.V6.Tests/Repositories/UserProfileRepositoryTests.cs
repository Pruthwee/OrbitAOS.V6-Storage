using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Infrastructure.Data;
using OrbitAOS.V6.Infrastructure.Repositories;
using Xunit;

namespace OrbitAOS.V6.Tests.Repositories;

/// <summary>
/// Integration-style unit tests for <see cref="UserProfileRepository"/>.
/// Uses EF Core InMemory provider to test repository operations without a real database.
/// </summary>
public class UserProfileRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserProfileRepository _repository;

    /// <summary>
    /// Initializes a fresh InMemory database context for each test.
    /// </summary>
    public UserProfileRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new UserProfileRepository(_context);
    }

    /// <summary>
    /// Verifies that AddAsync persists a new entity and returns it with a generated ID.
    /// </summary>
    [Fact]
    public async Task AddAsync_PersistsEntityAndReturnsWithId()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "identity-001",
            DisplayName = "Alice",
            Email = "alice@example.com",
            IsActive = true
        };

        // Act
        var result = await _repository.AddAsync(profile);

        // Assert
        Assert.True(result.Id > 0);
        Assert.Equal("alice@example.com", result.Email);
        Assert.Equal(1, await _context.UserProfiles.CountAsync());
    }

    /// <summary>
    /// Verifies that GetByIdAsync returns the correct entity.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectEntity()
    {
        // Arrange
        var profile = await SeedProfileAsync("bob@example.com", "Bob");

        // Act
        var result = await _repository.GetByIdAsync(profile.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("bob@example.com", result.Email);
    }

    /// <summary>
    /// Verifies that GetByIdAsync returns null for a non-existent ID.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        // Act
        var result = await _repository.GetByIdAsync(9999);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that GetAllAsync returns all seeded entities.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ReturnsAllEntities()
    {
        // Arrange
        await SeedProfileAsync("user1@example.com", "User One");
        await SeedProfileAsync("user2@example.com", "User Two");

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
    }

    /// <summary>
    /// Verifies that GetByIdentityUserIdAsync returns the correct profile.
    /// </summary>
    [Fact]
    public async Task GetByIdentityUserIdAsync_ReturnsCorrectProfile()
    {
        // Arrange
        await SeedProfileAsync("charlie@example.com", "Charlie", identityUserId: "identity-charlie");

        // Act
        var result = await _repository.GetByIdentityUserIdAsync("identity-charlie");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("charlie@example.com", result.Email);
    }

    /// <summary>
    /// Verifies that GetByEmailAsync returns the correct profile.
    /// </summary>
    [Fact]
    public async Task GetByEmailAsync_ReturnsCorrectProfile()
    {
        // Arrange
        await SeedProfileAsync("diana@example.com", "Diana");

        // Act
        var result = await _repository.GetByEmailAsync("diana@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Diana", result.DisplayName);
    }

    /// <summary>
    /// Verifies that DeleteAsync removes the entity from the database.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_RemovesEntityFromDatabase()
    {
        // Arrange
        var profile = await SeedProfileAsync("eve@example.com", "Eve");

        // Act
        await _repository.DeleteAsync(profile);

        // Assert
        Assert.Equal(0, await _context.UserProfiles.CountAsync());
    }

    /// <summary>
    /// Verifies that UpdateAsync persists changes to the entity.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_PersistsChanges()
    {
        // Arrange
        var profile = await SeedProfileAsync("frank@example.com", "Frank");
        profile.DisplayName = "Frank Updated";
        profile.UpdatedAt = DateTime.UtcNow;

        // Act
        await _repository.UpdateAsync(profile);

        // Assert
        var updated = await _repository.GetByIdAsync(profile.Id);
        Assert.Equal("Frank Updated", updated?.DisplayName);
    }

    // -------------------------------------------------------------------------
    // IDisposable
    // -------------------------------------------------------------------------

    /// <summary>Disposes the database context after each test.</summary>
    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private async Task<UserProfile> SeedProfileAsync(
        string email,
        string displayName,
        string? identityUserId = null)
    {
        var profile = new UserProfile
        {
            IdentityUserId = identityUserId ?? $"identity-{Guid.NewGuid()}",
            DisplayName = displayName,
            Email = email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();
        return profile;
    }
}
