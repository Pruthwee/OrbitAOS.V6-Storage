using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Infrastructure.Data;
using OrbitAOS.Infrastructure.Repositories;
using Xunit;

namespace OrbitAOS.Tests.Infrastructure;

/// <summary>
/// Integration tests for UserProfileRepository using EF Core InMemory provider.
/// Tests data access operations against an in-memory database.
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
    public async Task AddAsync_WithValidProfile_PersistsToDatabase()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "test-user-1",
            DisplayName = "Test User",
            Department = "Engineering",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddAsync(profile);

        // Assert
        Assert.NotNull(result);
        var saved = await _context.UserProfiles.FindAsync(result.Id);
        Assert.NotNull(saved);
        Assert.Equal("Test User", saved.DisplayName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsProfile()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "test-user-2",
            DisplayName = "Find Me",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _context.UserProfiles.AddAsync(profile);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(profile.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Find Me", result.DisplayName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(9999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdentityUserIdAsync_WhenExists_ReturnsProfile()
    {
        // Arrange
        var identityUserId = "identity-abc-123";
        var profile = new UserProfile
        {
            IdentityUserId = identityUserId,
            DisplayName = "Identity User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _context.UserProfiles.AddAsync(profile);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdentityUserIdAsync(identityUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(identityUserId, result.IdentityUserId);
    }

    [Fact]
    public async Task GetActiveUsersAsync_ReturnsOnlyActiveProfiles()
    {
        // Arrange
        await _context.UserProfiles.AddRangeAsync(
            new UserProfile { IdentityUserId = "u1", DisplayName = "Active 1", IsActive = true, CreatedAt = DateTime.UtcNow },
            new UserProfile { IdentityUserId = "u2", DisplayName = "Active 2", IsActive = true, CreatedAt = DateTime.UtcNow },
            new UserProfile { IdentityUserId = "u3", DisplayName = "Inactive", IsActive = false, CreatedAt = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActiveUsersAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, p => Assert.True(p.IsActive));
    }

    [Fact]
    public async Task UpdateAsync_WithValidProfile_PersistsChanges()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "update-user",
            DisplayName = "Original Name",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _context.UserProfiles.AddAsync(profile);
        await _context.SaveChangesAsync();

        // Act
        profile.DisplayName = "Updated Name";
        profile.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(profile);

        // Assert
        var updated = await _context.UserProfiles.FindAsync(profile.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated Name", updated.DisplayName);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact]
    public async Task DeleteAsync_WithValidProfile_RemovesFromDatabase()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "delete-user",
            DisplayName = "To Delete",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _context.UserProfiles.AddAsync(profile);
        await _context.SaveChangesAsync();
        var profileId = profile.Id;

        // Act
        await _repository.DeleteAsync(profile);

        // Assert
        var deleted = await _context.UserProfiles.FindAsync(profileId);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProfiles()
    {
        // Arrange
        await _context.UserProfiles.AddRangeAsync(
            new UserProfile { IdentityUserId = "all-1", DisplayName = "User 1", IsActive = true, CreatedAt = DateTime.UtcNow },
            new UserProfile { IdentityUserId = "all-2", DisplayName = "User 2", IsActive = false, CreatedAt = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
