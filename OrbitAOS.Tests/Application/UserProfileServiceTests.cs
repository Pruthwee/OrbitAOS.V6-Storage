using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.Application.Interfaces;
using OrbitAOS.Application.Services;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;
using Xunit;

namespace OrbitAOS.Tests.Application;

/// <summary>
/// Unit tests for UserProfileService application service.
/// Tests business logic in isolation using Moq for dependency mocking.
/// </summary>
public class UserProfileServiceTests
{
    private readonly Mock<IUserProfileRepository> _repositoryMock;
    private readonly Mock<ILogger<UserProfileService>> _loggerMock;
    private readonly IUserProfileService _service;

    public UserProfileServiceTests()
    {
        _repositoryMock = new Mock<IUserProfileRepository>();
        _loggerMock = new Mock<ILogger<UserProfileService>>();
        _service = new UserProfileService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProfileExists_ReturnsDto()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "user-123",
            DisplayName = "John Doe",
            Department = "Engineering",
            Position = "Developer",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John Doe", result.DisplayName);
        Assert.Equal("user-123", result.IdentityUserId);
        Assert.Equal("Engineering", result.Department);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProfileNotFound_ReturnsNull()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdentityUserIdAsync_WhenProfileExists_ReturnsDto()
    {
        // Arrange
        var identityUserId = "identity-user-abc";
        var profile = new UserProfile
        {
            IdentityUserId = identityUserId,
            DisplayName = "Jane Smith",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.GetByIdentityUserIdAsync(identityUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        // Act
        var result = await _service.GetByIdentityUserIdAsync(identityUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Jane Smith", result.DisplayName);
        Assert.Equal(identityUserId, result.IdentityUserId);
    }

    [Fact]
    public async Task GetAllActiveAsync_ReturnsOnlyActiveProfiles()
    {
        // Arrange
        var activeProfiles = new List<UserProfile>
        {
            new() { IdentityUserId = "u1", DisplayName = "Alice", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { IdentityUserId = "u2", DisplayName = "Bob", IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        _repositoryMock
            .Setup(r => r.GetActiveUsersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(activeProfiles.AsReadOnly());

        // Act
        var result = await _service.GetAllActiveAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, dto => Assert.True(dto.IsActive));
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_CreatesAndReturnsDto()
    {
        // Arrange
        var request = new CreateUserProfileRequest(
            IdentityUserId: "new-user-id",
            DisplayName: "New User",
            Department: "HR",
            Position: "Manager");

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<UserProfile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile entity, CancellationToken _) => entity);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("new-user-id", result.IdentityUserId);
        Assert.Equal("New User", result.DisplayName);
        Assert.Equal("HR", result.Department);
        Assert.Equal("Manager", result.Position);
        Assert.True(result.IsActive);

        _repositoryMock.Verify(
            r => r.AddAsync(It.Is<UserProfile>(p =>
                p.IdentityUserId == "new-user-id" &&
                p.DisplayName == "New User" &&
                p.IsActive == true),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenProfileExists_UpdatesSuccessfully()
    {
        // Arrange
        var existingProfile = new UserProfile
        {
            IdentityUserId = "user-456",
            DisplayName = "Old Name",
            Department = "Old Dept",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var updateRequest = new UpdateUserProfileRequest(
            DisplayName: "New Name",
            Department: "New Dept",
            Position: "Senior Developer");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProfile);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<UserProfile>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updateRequest);

        // Assert
        _repositoryMock.Verify(
            r => r.UpdateAsync(It.Is<UserProfile>(p =>
                p.DisplayName == "New Name" &&
                p.Department == "New Dept" &&
                p.Position == "Senior Developer"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenProfileNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        var updateRequest = new UpdateUserProfileRequest("Name", null, null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.UpdateAsync(999, updateRequest));
    }

    [Fact]
    public async Task DeactivateAsync_WhenProfileExists_SetsIsActiveFalse()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "user-789",
            DisplayName = "Active User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<UserProfile>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeactivateAsync(1);

        // Assert
        _repositoryMock.Verify(
            r => r.UpdateAsync(It.Is<UserProfile>(p => p.IsActive == false),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeactivateAsync_WhenProfileNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.DeactivateAsync(999));
    }
}
