using FluentAssertions;
using Moq;
using OrbitAOS.Application.DTOs;
using OrbitAOS.Application.Services;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;

namespace OrbitAOS.Tests.Application;

/// <summary>
/// Unit tests for <see cref="UserProfileService"/>.
/// Tests business logic in isolation using mocked dependencies (Moq).
/// </summary>
public class UserProfileServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserProfileRepository> _repoMock;
    private readonly UserProfileService _sut;

    public UserProfileServiceTests()
    {
        _repoMock = new Mock<IUserProfileRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.UserProfiles).Returns(_repoMock.Object);
        _sut = new UserProfileService(_unitOfWorkMock.Object);
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ReturnsMappedDto()
    {
        // Arrange
        var entity = new UserProfile
        {
            Id = 1,
            IdentityUserId = "user-id-1",
            DisplayName = "Alice",
            Email = "alice@example.com",
            IsActive = true,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        _repoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(entity);

        // Act
        var result = await _sut.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.DisplayName.Should().Be("Alice");
        result.Email.Should().Be("alice@example.com");
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityDoesNotExist_ReturnsNull()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((UserProfile?)null);

        // Act
        var result = await _sut.GetByIdAsync(99);

        // Assert
        result.Should().BeNull();
    }

    // ── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsAllMappedDtos()
    {
        // Arrange
        var entities = new List<UserProfile>
        {
            new() { Id = 1, IdentityUserId = "uid1", DisplayName = "Alice", Email = "alice@example.com", IsActive = true },
            new() { Id = 2, IdentityUserId = "uid2", DisplayName = "Bob",   Email = "bob@example.com",   IsActive = false }
        };
        _repoMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(entities.AsReadOnly());

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].DisplayName.Should().Be("Alice");
        result[1].DisplayName.Should().Be("Bob");
    }

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_AddsEntityAndSavesChanges()
    {
        // Arrange
        var dto = new UserProfileDto
        {
            IdentityUserId = "new-uid",
            DisplayName = "Charlie",
            Email = "charlie@example.com",
            IsActive = true
        };

        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<UserProfile>(), default))
            .ReturnsAsync((UserProfile p, CancellationToken _) =>
            {
                p.Id = 10;
                return p;
            });

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _sut.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(10);
        result.DisplayName.Should().Be("Charlie");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    // ── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WhenEntityExists_UpdatesAndSaves()
    {
        // Arrange
        var existing = new UserProfile
        {
            Id = 5,
            IdentityUserId = "uid5",
            DisplayName = "Old Name",
            Email = "old@example.com",
            IsActive = true
        };
        _repoMock.Setup(r => r.GetByIdAsync(5, default)).ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        var dto = new UserProfileDto
        {
            Id = 5,
            IdentityUserId = "uid5",
            DisplayName = "New Name",
            Email = "new@example.com",
            IsActive = false
        };

        // Act
        await _sut.UpdateAsync(dto);

        // Assert
        existing.DisplayName.Should().Be("New Name");
        existing.Email.Should().Be("new@example.com");
        existing.IsActive.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((UserProfile?)null);

        var dto = new UserProfileDto { Id = 999, DisplayName = "Ghost", Email = "ghost@example.com" };

        // Act
        Func<Task> act = () => _sut.UpdateAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*999*");
    }

    // ── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_CallsRepositoryDeleteAndSaves()
    {
        // Arrange
        _repoMock.Setup(r => r.DeleteAsync(3, default)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        await _sut.DeleteAsync(3);

        // Assert
        _repoMock.Verify(r => r.DeleteAsync(3, default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }
}
