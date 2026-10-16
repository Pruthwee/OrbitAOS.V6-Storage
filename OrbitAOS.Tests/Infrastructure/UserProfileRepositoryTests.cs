using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Infrastructure.Data;
using OrbitAOS.Infrastructure.Repositories;

namespace OrbitAOS.Tests.Infrastructure;

/// <summary>
/// Integration-style unit tests for <see cref="UserProfileRepository"/>
/// using EF Core InMemory provider to avoid a real database dependency.
/// </summary>
public class UserProfileRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserProfileRepository _sut;

    public UserProfileRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _sut = new UserProfileRepository(_context);
    }

    // ── AddAsync / GetByIdAsync ──────────────────────────────────────────────

    [Fact]
    public async Task AddAsync_ThenGetByIdAsync_ReturnsAddedEntity()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "uid-1",
            DisplayName = "Alice",
            Email = "alice@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _sut.AddAsync(profile);
        await _context.SaveChangesAsync();

        var retrieved = await _sut.GetByIdAsync(profile.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.DisplayName.Should().Be("Alice");
        retrieved.Email.Should().Be("alice@example.com");
    }

    // ── GetByIdentityUserIdAsync ─────────────────────────────────────────────

    [Fact]
    public async Task GetByIdentityUserIdAsync_WhenExists_ReturnsEntity()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "uid-abc",
            DisplayName = "Bob",
            Email = "bob@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _sut.AddAsync(profile);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByIdentityUserIdAsync("uid-abc");

        // Assert
        result.Should().NotBeNull();
        result!.DisplayName.Should().Be("Bob");
    }

    [Fact]
    public async Task GetByIdentityUserIdAsync_WhenNotExists_ReturnsNull()
    {
        // Act
        var result = await _sut.GetByIdentityUserIdAsync("non-existent-uid");

        // Assert
        result.Should().BeNull();
    }

    // ── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsAllProfiles()
    {
        // Arrange
        await _sut.AddAsync(new UserProfile { IdentityUserId = "u1", DisplayName = "Alice", Email = "a@a.com", CreatedAt = DateTime.UtcNow });
        await _sut.AddAsync(new UserProfile { IdentityUserId = "u2", DisplayName = "Bob",   Email = "b@b.com", CreatedAt = DateTime.UtcNow });
        await _context.SaveChangesAsync();

        // Act
        var all = await _sut.GetAllAsync();

        // Assert
        all.Should().HaveCount(2);
    }

    // ── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_PersistsChanges()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "uid-upd",
            DisplayName = "Original",
            Email = "orig@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _sut.AddAsync(profile);
        await _context.SaveChangesAsync();

        // Act
        profile.DisplayName = "Updated";
        await _sut.UpdateAsync(profile);
        await _context.SaveChangesAsync();

        var updated = await _sut.GetByIdAsync(profile.Id);

        // Assert
        updated!.DisplayName.Should().Be("Updated");
    }

    // ── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_RemovesEntity()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "uid-del",
            DisplayName = "ToDelete",
            Email = "del@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _sut.AddAsync(profile);
        await _context.SaveChangesAsync();

        // Act
        await _sut.DeleteAsync(profile.Id);
        await _context.SaveChangesAsync();

        var deleted = await _sut.GetByIdAsync(profile.Id);

        // Assert
        deleted.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
