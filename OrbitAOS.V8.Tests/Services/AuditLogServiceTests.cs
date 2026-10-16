using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V8.Application.Services;
using OrbitAOS.V8.Domain.Entities;
using OrbitAOS.V8.Domain.Interfaces;
using Xunit;

namespace OrbitAOS.V8.Tests.Services;

/// <summary>
/// Unit tests for AuditLogService.
/// </summary>
public class AuditLogServiceTests
{
    private readonly Mock<IAuditLogRepository> _repositoryMock;
    private readonly Mock<ILogger<AuditLogService>> _loggerMock;
    private readonly AuditLogService _service;

    public AuditLogServiceTests()
    {
        _repositoryMock = new Mock<IAuditLogRepository>();
        _loggerMock = new Mock<ILogger<AuditLogService>>();
        _service = new AuditLogService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task LogAsync_CreatesAuditLogEntry()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<AuditLog>(), default))
            .ReturnsAsync((AuditLog log, CancellationToken _) =>
            {
                log.Id = 1;
                return log;
            });

        // Act
        await _service.LogAsync(
            action: "Login",
            entityType: "AccountController",
            userId: "user-123",
            ipAddress: "127.0.0.1");

        // Assert
        _repositoryMock.Verify(r => r.AddAsync(
            It.Is<AuditLog>(l =>
                l.Action == "Login" &&
                l.EntityType == "AccountController" &&
                l.UserId == "user-123" &&
                l.IpAddress == "127.0.0.1"),
            default), Times.Once);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsMappedDtos()
    {
        // Arrange
        var logs = new List<AuditLog>
        {
            new() { Id = 1, Action = "Login", EntityType = "Account", UserId = "user-1", CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Action = "Update", EntityType = "Profile", UserId = "user-1", CreatedAt = DateTime.UtcNow }
        };
        _repositoryMock.Setup(r => r.GetByUserIdAsync("user-1", default)).ReturnsAsync(logs.AsReadOnly());

        // Act
        var result = await _service.GetByUserIdAsync("user-1");

        // Assert
        result.Should().HaveCount(2);
        result[0].Action.Should().Be("Login");
        result[1].Action.Should().Be("Update");
    }

    [Fact]
    public async Task GetByEntityTypeAsync_ReturnsMappedDtos()
    {
        // Arrange
        var logs = new List<AuditLog>
        {
            new() { Id = 3, Action = "Create", EntityType = "UserProfile", UserId = "user-2", CreatedAt = DateTime.UtcNow }
        };
        _repositoryMock.Setup(r => r.GetByEntityTypeAsync("UserProfile", default)).ReturnsAsync(logs.AsReadOnly());

        // Act
        var result = await _service.GetByEntityTypeAsync("UserProfile");

        // Assert
        result.Should().HaveCount(1);
        result[0].EntityType.Should().Be("UserProfile");
    }
}
