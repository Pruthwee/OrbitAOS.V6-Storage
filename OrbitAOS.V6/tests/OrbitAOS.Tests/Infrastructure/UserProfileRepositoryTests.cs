using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Infrastructure.Data;
using OrbitAOS.Infrastructure.Repositories;
using FluentAssertions;

namespace OrbitAOS.Tests.Infrastructure;

/// <summary>
/// Integration tests for <see cref="UserProfileRepository"/> using EF Core InMemory database.
/// Tests repository operations against an in-memory database to verify data access logic.
/// </summary>
public class UserProfileRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserProfileRepository _repository;

    /// <summary>
    /// Initializes test fixtures with an in-memory EF Core database.
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
    /// Tests that AddAsync persists a new entity to the database.
    /// </summary>
    [Fact]
    public async Task AddAsync_PersistsEntity()
    {
        // Arrange
        var profile = CreateSampleProfile("identity-1", "user1@test.com", "User One");

        // Act
        var result = await _repository.AddAsync(profile);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        var saved = await _context.UserProfiles.FindAsync(result.Id);
        saved.Should().NotBeNull();
        saved!.Email.Should().Be("user1@test.com");
    }

    /// <summary>
    /// Tests that GetByIdAsync retrieves an existing entity.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenExists()
    {
        // Arrange
        var profile = CreateSampleProfile("identity-2", "user2@test.com", "User Two");
        await _context.UserProfiles.AddAsync(profile);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(profile.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("user2@test.com");
    }

    /// <summary>
    /// Tests that GetByIdAsync returns null when entity does not exist.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        // Act
        var result = await _repository.GetByIdAsync(9999);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Tests that GetAllAsync returns all persisted entities.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ReturnsAllEntities()
    {
        // Arrange
        await _context.UserProfiles.AddRangeAsync(
            CreateSampleProfile("id-1", "a@test.com", "User A"),
            CreateSampleProfile("id-2", "b@test.com", "User B"),
            CreateSampleProfile("id-3", "c@test.com", "User C"));
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    /// <summary>
    /// Tests that UpdateAsync modifies an existing entity.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ModifiesEntity()
    {
        // Arrange
        var profile = CreateSampleProfile("identity-3", "user3@test.com", "Original Name");
        await _context.UserProfiles.AddAsync(profile);
        await _context.SaveChangesAsync();

        // Act
        profile.DisplayName = "Updated Name";
        await _repository.UpdateAsync(profile);

        // Assert
        var updated = await _context.UserProfiles.FindAsync(profile.Id);
        updated!.DisplayName.Should().Be("Updated Name");
        updated.UpdatedAt.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that DeleteAsync removes an entity from the database.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_RemovesEntity()
    {
        // Arrange
        var profile = CreateSampleProfile("identity-4", "user4@test.com", "User Four");
        await _context.UserProfiles.AddAsync(profile);
        await _context.SaveChangesAsync();
        var id = profile.Id;

        // Act
        await _repository.DeleteAsync(id);

        // Assert
        var deleted = await _context.UserProfiles.FindAsync(id);
        deleted.Should().BeNull();
    }

    /// <summary>
    /// Tests that ExistsAsync returns true for an existing entity.
    /// </summary>
    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenEntityExists()
    {
        // Arrange
        var profile = CreateSampleProfile("identity-5", "user5@test.com", "User Five");
        await _context.UserProfiles.AddAsync(profile);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _repository.ExistsAsync(profile.Id);

        // Assert
        exists.Should().BeTrue();
    }

    /// <summary>
    /// Tests that ExistsAsync returns false for a non-existent entity.
    /// </summary>
    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenEntityNotFound()
    {
        // Act
        var exists = await _repository.ExistsAsync(9999);

        // Assert
        exists.Should().BeFalse();
    }

    /// <summary>
    /// Tests that GetByIdentityUserIdAsync retrieves the correct profile.
    /// </summary>
    [Fact]
    public async Task GetByIdentityUserIdAsync_ReturnsCorrectProfile()
    {
        // Arrange
        var profile = CreateSampleProfile("unique-identity-id", "unique@test.com", "Unique User");
        await _context.UserProfiles.AddAsync(profile);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdentityUserIdAsync("unique-identity-id");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("unique@test.com");
    }

    /// <summary>
    /// Tests that GetByEmailAsync retrieves the correct profile.
    /// </summary>
    [Fact]
    public async Task GetByEmailAsync_ReturnsCorrectProfile()
    {
        // Arrange
        var profile = CreateSampleProfile("identity-email", "findme@test.com", "Find Me");
        await _context.UserProfiles.AddAsync(profile);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync("findme@test.com");

        // Assert
        result.Should().NotBeNull();
        result!.DisplayName.Should().Be("Find Me");
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    // ─── Private Helpers ─────────────────────────────────────────────────────

    private static UserProfile CreateSampleProfile(
        string identityUserId,
        string email,
        string displayName) => new()
    {
        IdentityUserId = identityUserId,
        Email = email,
        DisplayName = displayName,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };
}
