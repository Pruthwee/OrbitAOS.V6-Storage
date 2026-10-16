using Moq;
using OrbitAOS.Application.DTOs;
using OrbitAOS.Application.Services;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;
using Xunit;

namespace OrbitAOS.Tests.Services;

/// <summary>
/// Unit tests for the UserProfileService application service.
/// Tests verify business logic, validation, and repository interactions.
/// </summary>
public class UserProfileServiceTests
{
    private readonly Mock<IUserProfileRepository> _mockRepository;
    private readonly UserProfileService _service;

    /// <summary>Initializes test fixtures for UserProfileService tests.</summary>
    public UserProfileServiceTests()
    {
        _mockRepository = new Mock<IUserProfileRepository>();
        _service = new UserProfileService(_mockRepository.Object);
    }

    // ─── GetByIdAsync Tests ───────────────────────────────────────────────────

    /// <summary>Verifies that GetByIdAsync returns a successful result when the profile exists.</summary>
    [Fact]
    public async Task GetByIdAsync_ReturnsSuccess_WhenProfileExists()
    {
        // Arrange
        var profile = CreateSampleProfile(1);
        _mockRepository.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(profile);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value);
        Assert.Equal(1, result.Value.Id);
        Assert.Equal("test@example.com", result.Value.Email);
    }

    /// <summary>Verifies that GetByIdAsync returns a failure result when the profile does not exist.</summary>
    [Fact]
    public async Task GetByIdAsync_ReturnsFailure_WhenProfileNotFound()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, default))
            .ReturnsAsync((UserProfile?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("999", result.Errors.First());
    }

    // ─── GetByIdentityUserIdAsync Tests ──────────────────────────────────────

    /// <summary>Verifies that GetByIdentityUserIdAsync returns failure for empty identity user ID.</summary>
    [Fact]
    public async Task GetByIdentityUserIdAsync_ReturnsFailure_WhenIdentityUserIdIsEmpty()
    {
        // Act
        var result = await _service.GetByIdentityUserIdAsync(string.Empty);

        // Assert
        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Errors);
    }

    /// <summary>Verifies that GetByIdentityUserIdAsync returns success when profile is found.</summary>
    [Fact]
    public async Task GetByIdentityUserIdAsync_ReturnsSuccess_WhenProfileFound()
    {
        // Arrange
        var identityId = "identity-user-id-123";
        var profile = CreateSampleProfile(1, identityId);
        _mockRepository.Setup(r => r.GetByIdentityUserIdAsync(identityId, default))
            .ReturnsAsync(profile);

        // Act
        var result = await _service.GetByIdentityUserIdAsync(identityId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(identityId, result.Value?.IdentityUserId);
    }

    // ─── GetAllAsync Tests ────────────────────────────────────────────────────

    /// <summary>Verifies that GetAllAsync returns all profiles from the repository.</summary>
    [Fact]
    public async Task GetAllAsync_ReturnsAllProfiles()
    {
        // Arrange
        var profiles = new List<UserProfile>
        {
            CreateSampleProfile(1),
            CreateSampleProfile(2, "identity-2", "user2@example.com"),
            CreateSampleProfile(3, "identity-3", "user3@example.com")
        };
        _mockRepository.Setup(r => r.GetAllAsync(default))
            .ReturnsAsync(profiles.AsReadOnly());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(3, result.Value?.Count);
    }

    // ─── CreateAsync Tests ────────────────────────────────────────────────────

    /// <summary>Verifies that CreateAsync returns failure when DTO is null.</summary>
    [Fact]
    public async Task CreateAsync_ReturnsFailure_WhenDtoIsNull()
    {
        // Act
        var result = await _service.CreateAsync(null!);

        // Assert
        Assert.False(result.Succeeded);
    }

    /// <summary>Verifies that CreateAsync returns failure when email is empty.</summary>
    [Fact]
    public async Task CreateAsync_ReturnsFailure_WhenEmailIsEmpty()
    {
        // Arrange
        var dto = new UserProfileDto { Email = string.Empty };

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Errors);
    }

    /// <summary>Verifies that CreateAsync returns success and calls repository when DTO is valid.</summary>
    [Fact]
    public async Task CreateAsync_ReturnsSuccess_WhenDtoIsValid()
    {
        // Arrange
        var dto = new UserProfileDto
        {
            IdentityUserId = "identity-new",
            DisplayName = "New User",
            Email = "newuser@example.com",
            IsActive = true
        };
        var createdProfile = CreateSampleProfile(10, dto.IdentityUserId, dto.Email);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<UserProfile>(), default))
            .ReturnsAsync(createdProfile);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.True(result.Succeeded);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<UserProfile>(), default), Times.Once);
    }

    // ─── UpdateAsync Tests ────────────────────────────────────────────────────

    /// <summary>Verifies that UpdateAsync returns failure when profile does not exist.</summary>
    [Fact]
    public async Task UpdateAsync_ReturnsFailure_WhenProfileNotFound()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, default))
            .ReturnsAsync((UserProfile?)null);

        var dto = new UserProfileDto { Id = 999, Email = "test@example.com" };

        // Act
        var result = await _service.UpdateAsync(dto);

        // Assert
        Assert.False(result.Succeeded);
    }

    /// <summary>Verifies that UpdateAsync returns success and calls repository when profile exists.</summary>
    [Fact]
    public async Task UpdateAsync_ReturnsSuccess_WhenProfileExists()
    {
        // Arrange
        var existing = CreateSampleProfile(1);
        _mockRepository.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(existing);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<UserProfile>(), default))
            .Returns(Task.CompletedTask);

        var dto = new UserProfileDto
        {
            Id = 1,
            DisplayName = "Updated Name",
            Email = "updated@example.com",
            IsActive = true
        };

        // Act
        var result = await _service.UpdateAsync(dto);

        // Assert
        Assert.True(result.Succeeded);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<UserProfile>(), default), Times.Once);
    }

    // ─── DeleteAsync Tests ────────────────────────────────────────────────────

    /// <summary>Verifies that DeleteAsync returns failure when profile does not exist.</summary>
    [Fact]
    public async Task DeleteAsync_ReturnsFailure_WhenProfileNotFound()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(999, default))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeleteAsync(999);

        // Assert
        Assert.False(result.Succeeded);
    }

    /// <summary>Verifies that DeleteAsync returns success and calls repository when profile exists.</summary>
    [Fact]
    public async Task DeleteAsync_ReturnsSuccess_WhenProfileExists()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(1, default))
            .ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync(1, default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        Assert.True(result.Succeeded);
        _mockRepository.Verify(r => r.DeleteAsync(1, default), Times.Once);
    }

    // ─── Helper Methods ───────────────────────────────────────────────────────

    /// <summary>Creates a sample UserProfile entity for testing.</summary>
    private static UserProfile CreateSampleProfile(
        int id,
        string identityUserId = "identity-user-id-1",
        string email = "test@example.com") => new()
    {
        Id = id,
        IdentityUserId = identityUserId,
        DisplayName = "Test User",
        Email = email,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };
}
