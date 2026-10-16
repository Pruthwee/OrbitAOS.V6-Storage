using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V8.Application.DTOs;
using OrbitAOS.V8.Application.Services;
using OrbitAOS.V8.Domain.Entities;
using OrbitAOS.V8.Domain.Interfaces;
using Xunit;

namespace OrbitAOS.V8.Tests.Services;

/// <summary>
/// Unit tests for UserProfileService.
/// 
/// Migration notes from legacy ASP.NET MVC 6.0 testing:
/// - Tests the Application layer service in isolation
/// - Uses Moq to mock repository dependencies
/// - No database required for service unit tests
/// - Async test methods with async/await
/// </summary>
public class UserProfileServiceTests
{
    private readonly Mock<IUserProfileRepository> _repositoryMock;
    private readonly Mock<ILogger<UserProfileService>> _loggerMock;
    private readonly UserProfileService _service;

    public UserProfileServiceTests()
    {
        _repositoryMock = new Mock<IUserProfileRepository>();
        _loggerMock = new Mock<ILogger<UserProfileService>>();
        _service = new UserProfileService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenEntityExists()
    {
        // Arrange
        var entity = new UserProfile
        {
            Id = 1,
            IdentityUserId = "user-123",
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(entity);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.DisplayName.Should().Be("Test User");
        result.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenEntityNotFound()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((UserProfile?)null);

        // Act
        var result = await _service.GetByIdAsync(99);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_AddsEntity_AndReturnsDto()
    {
        // Arrange
        var dto = new UserProfileDto
        {
            IdentityUserId = "user-456",
            DisplayName = "New User",
            Email = "new@example.com"
        };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<UserProfile>(), default))
            .ReturnsAsync((UserProfile entity, CancellationToken _) =>
            {
                entity.Id = 2;
                return entity;
            });

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(2);
        result.DisplayName.Should().Be("New User");
        result.IsActive.Should().BeTrue();

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<UserProfile>(), default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsInvalidOperationException_WhenEntityNotFound()
    {
        // Arrange
        var dto = new UserProfileDto { Id = 999, DisplayName = "Ghost", Email = "ghost@example.com" };
        _repositoryMock.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((UserProfile?)null);

        // Act
        var act = async () => await _service.UpdateAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*999*");
    }

    [Fact]
    public async Task UpdateAsync_UpdatesEntity_WhenEntityExists()
    {
        // Arrange
        var entity = new UserProfile
        {
            Id = 3,
            IdentityUserId = "user-789",
            DisplayName = "Old Name",
            Email = "old@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        var dto = new UserProfileDto
        {
            Id = 3,
            DisplayName = "New Name",
            Email = "new@example.com",
            IsActive = true
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(3, default)).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<UserProfile>(), default)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(dto);

        // Assert
        _repositoryMock.Verify(r => r.UpdateAsync(
            It.Is<UserProfile>(u => u.DisplayName == "New Name" && u.Email == "new@example.com"),
            default), Times.Once);
    }

    [Fact]
    public async Task DeactivateAsync_SetsIsActive_ToFalse()
    {
        // Arrange
        var entity = new UserProfile
        {
            Id = 4,
            IdentityUserId = "user-101",
            DisplayName = "Active User",
            Email = "active@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(4, default)).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<UserProfile>(), default)).Returns(Task.CompletedTask);

        // Act
        await _service.DeactivateAsync(4);

        // Assert
        _repositoryMock.Verify(r => r.UpdateAsync(
            It.Is<UserProfile>(u => u.IsActive == false),
            default), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDtos()
    {
        // Arrange
        var entities = new List<UserProfile>
        {
            new() { Id = 1, IdentityUserId = "u1", DisplayName = "User 1", Email = "u1@test.com", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, IdentityUserId = "u2", DisplayName = "User 2", Email = "u2@test.com", IsActive = false, CreatedAt = DateTime.UtcNow }
        };
        _repositoryMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(entities.AsReadOnly());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].DisplayName.Should().Be("User 1");
        result[1].IsActive.Should().BeFalse();
    }
}
