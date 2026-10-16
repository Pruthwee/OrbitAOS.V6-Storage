using Moq;
using OrbitAOS.Application.DTOs;
using OrbitAOS.Application.Interfaces;
using OrbitAOS.Application.Services;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;
using FluentAssertions;
using Xunit;

namespace OrbitAOS.Tests.Services;

/// <summary>
/// Unit tests for <see cref="UserProfileService"/>.
/// Tests business logic in isolation using mocked repositories.
/// </summary>
public class UserProfileServiceTests
{
    private readonly Mock<IUserProfileRepository> _repositoryMock;
    private readonly IUserProfileService _service;

    /// <summary>
    /// Initializes test fixtures for UserProfileService tests.
    /// </summary>
    public UserProfileServiceTests()
    {
        _repositoryMock = new Mock<IUserProfileRepository>();
        _service = new UserProfileService(_repositoryMock.Object);
    }

    /// <summary>
    /// Tests that GetByIdAsync returns a DTO when entity exists.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenEntityExists()
    {
        // Arrange
        var entity = CreateSampleUserProfile(1);
        _repositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.DisplayName.Should().Be("Test User");
        result.Email.Should().Be("test@example.com");
    }

    /// <summary>
    /// Tests that GetByIdAsync returns null when entity does not exist.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenEntityNotFound()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Tests that GetAllAsync returns all user profiles as DTOs.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ReturnsAllProfiles()
    {
        // Arrange
        var entities = new List<UserProfile>
        {
            CreateSampleUserProfile(1),
            CreateSampleUserProfile(2, "user2@example.com", "User Two")
        };
        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        var list = result.ToList();
        list.Should().HaveCount(2);
        list[0].Id.Should().Be(1);
        list[1].Id.Should().Be(2);
    }

    /// <summary>
    /// Tests that CreateAsync calls repository AddAsync and returns created DTO.
    /// </summary>
    [Fact]
    public async Task CreateAsync_CallsRepositoryAndReturnsDto()
    {
        // Arrange
        var dto = new UserProfileDto
        {
            IdentityUserId = "identity-123",
            DisplayName = "New User",
            Email = "new@example.com",
            IsActive = true
        };
        var createdEntity = CreateSampleUserProfile(10, "new@example.com", "New User");
        createdEntity.IdentityUserId = "identity-123";

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<UserProfile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdEntity);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(10);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<UserProfile>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that UpdateAsync calls repository UpdateAsync.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_CallsRepositoryUpdate()
    {
        // Arrange
        var dto = new UserProfileDto { Id = 1, DisplayName = "Updated", Email = "u@e.com" };
        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<UserProfile>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(dto);

        // Assert
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserProfile>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that DeleteAsync calls repository DeleteAsync.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_CallsRepositoryDelete()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that GetByIdentityUserIdAsync returns DTO when profile exists.
    /// </summary>
    [Fact]
    public async Task GetByIdentityUserIdAsync_ReturnsDto_WhenFound()
    {
        // Arrange
        var entity = CreateSampleUserProfile(1);
        entity.IdentityUserId = "identity-abc";
        _repositoryMock
            .Setup(r => r.GetByIdentityUserIdAsync("identity-abc", It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _service.GetByIdentityUserIdAsync("identity-abc");

        // Assert
        result.Should().NotBeNull();
        result!.IdentityUserId.Should().Be("identity-abc");
    }

    // ─── Private Helpers ─────────────────────────────────────────────────────

    private static UserProfile CreateSampleUserProfile(
        int id,
        string email = "test@example.com",
        string displayName = "Test User") => new()
    {
        Id = id,
        IdentityUserId = $"identity-{id}",
        DisplayName = displayName,
        Email = email,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };
}
