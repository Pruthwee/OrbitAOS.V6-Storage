using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Application.Services;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Domain.Interfaces;
using Xunit;

namespace OrbitAOS.V6.Tests.Services;

/// <summary>
/// Unit tests for <see cref="UserProfileService"/>.
/// Tests business logic in isolation using mocked repository and logger.
/// </summary>
public class UserProfileServiceTests
{
    private readonly Mock<IUserProfileRepository> _repositoryMock;
    private readonly Mock<ILogger<UserProfileService>> _loggerMock;
    private readonly UserProfileService _service;

    /// <summary>
    /// Initializes test fixtures with mocked dependencies.
    /// </summary>
    public UserProfileServiceTests()
    {
        _repositoryMock = new Mock<IUserProfileRepository>();
        _loggerMock = new Mock<ILogger<UserProfileService>>();
        _service = new UserProfileService(_repositoryMock.Object, _loggerMock.Object);
    }

    // -------------------------------------------------------------------------
    // GetByIdAsync tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Verifies that GetByIdAsync returns a DTO when the entity exists.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenEntityExists()
    {
        // Arrange
        var entity = CreateSampleUserProfile(id: 1);
        _repositoryMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(entity);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("Test User", result.DisplayName);
    }

    /// <summary>
    /// Verifies that GetByIdAsync returns null when the entity does not exist.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenEntityNotFound()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((UserProfile?)null);

        // Act
        var result = await _service.GetByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    // -------------------------------------------------------------------------
    // GetAllAsync tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Verifies that GetAllAsync returns all profiles as DTOs.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ReturnsAllProfiles()
    {
        // Arrange
        var entities = new List<UserProfile>
        {
            CreateSampleUserProfile(id: 1, email: "user1@example.com"),
            CreateSampleUserProfile(id: 2, email: "user2@example.com")
        };
        _repositoryMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(entities.AsReadOnly());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, dto => dto.Email == "user1@example.com");
        Assert.Contains(result, dto => dto.Email == "user2@example.com");
    }

    /// <summary>
    /// Verifies that GetAllAsync returns an empty list when no profiles exist.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoProfiles()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync(default))
            .ReturnsAsync(new List<UserProfile>().AsReadOnly());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    // -------------------------------------------------------------------------
    // CreateAsync tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Verifies that CreateAsync calls repository AddAsync and returns the created DTO.
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
        var createdEntity = CreateSampleUserProfile(id: 5, email: "new@example.com");
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<UserProfile>(), default))
            .ReturnsAsync(createdEntity);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<UserProfile>(), default), Times.Once);
    }

    // -------------------------------------------------------------------------
    // UpdateAsync tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Verifies that UpdateAsync calls repository UpdateAsync when entity exists.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_CallsRepositoryUpdate_WhenEntityExists()
    {
        // Arrange
        var existing = CreateSampleUserProfile(id: 1);
        _repositoryMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<UserProfile>(), default))
            .Returns(Task.CompletedTask);

        var dto = new UserProfileDto
        {
            Id = 1,
            DisplayName = "Updated Name",
            Email = "updated@example.com",
            IsActive = false
        };

        // Act
        await _service.UpdateAsync(dto);

        // Assert
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserProfile>(), default), Times.Once);
    }

    /// <summary>
    /// Verifies that UpdateAsync throws InvalidOperationException when entity not found.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ThrowsInvalidOperationException_WhenEntityNotFound()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((UserProfile?)null);

        var dto = new UserProfileDto { Id = 99 };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(dto));
    }

    // -------------------------------------------------------------------------
    // DeleteAsync tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Verifies that DeleteAsync calls repository DeleteAsync when entity exists.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_CallsRepositoryDelete_WhenEntityExists()
    {
        // Arrange
        var entity = CreateSampleUserProfile(id: 1);
        _repositoryMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.DeleteAsync(It.IsAny<UserProfile>(), default))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<UserProfile>(), default), Times.Once);
    }

    /// <summary>
    /// Verifies that DeleteAsync throws InvalidOperationException when entity not found.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ThrowsInvalidOperationException_WhenEntityNotFound()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((UserProfile?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteAsync(99));
    }

    // -------------------------------------------------------------------------
    // Constructor tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Verifies that constructor throws ArgumentNullException when repository is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new UserProfileService(null!, _loggerMock.Object));
    }

    /// <summary>
    /// Verifies that constructor throws ArgumentNullException when logger is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new UserProfileService(_repositoryMock.Object, null!));
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private static UserProfile CreateSampleUserProfile(
        int id = 1,
        string email = "test@example.com",
        string displayName = "Test User")
        => new()
        {
            Id = id,
            IdentityUserId = $"identity-{id}",
            DisplayName = displayName,
            Email = email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
}
