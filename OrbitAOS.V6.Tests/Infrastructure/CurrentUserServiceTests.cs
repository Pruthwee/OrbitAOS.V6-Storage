// ============================================================
// Tests/Infrastructure/CurrentUserServiceTests.cs
// Unit Tests - Infrastructure Services
// Tests for CurrentUserService (replaces HttpContext.Current.User)
// ============================================================

using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OrbitAOS.V6.Infrastructure.Services;
using System.Security.Claims;
using Xunit;

namespace OrbitAOS.V6.Tests.Infrastructure;

/// <summary>
/// Unit tests for CurrentUserService.
/// Demonstrates testing the IHttpContextAccessor pattern in .NET 10.
/// </summary>
public class CurrentUserServiceTests
{
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly CurrentUserService _service;

    public CurrentUserServiceTests()
    {
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _service = new CurrentUserService(_mockHttpContextAccessor.Object);
    }

    [Fact]
    public void UserId_ShouldReturnNameIdentifierClaim_WhenAuthenticated()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "user-id-123"),
            new Claim(ClaimTypes.Name, "testuser@example.com")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = principal };
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var userId = _service.UserId;

        // Assert
        userId.Should().Be("user-id-123");
    }

    [Fact]
    public void IsAuthenticated_ShouldReturnTrue_WhenUserIsAuthenticated()
    {
        // Arrange
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "user@example.com") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = principal };
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var isAuthenticated = _service.IsAuthenticated;

        // Assert
        isAuthenticated.Should().BeTrue();
    }

    [Fact]
    public void IsAuthenticated_ShouldReturnFalse_WhenHttpContextIsNull()
    {
        // Arrange
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        // Act
        var isAuthenticated = _service.IsAuthenticated;

        // Assert
        isAuthenticated.Should().BeFalse();
    }

    [Fact]
    public void UserId_ShouldReturnNull_WhenNotAuthenticated()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var userId = _service.UserId;

        // Assert
        userId.Should().BeNull();
    }

    [Fact]
    public void IsInRole_ShouldReturnTrue_WhenUserHasRole()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "admin@example.com"),
            new Claim(ClaimTypes.Role, "Administrator")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = principal };
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var isInRole = _service.IsInRole("Administrator");

        // Assert
        isInRole.Should().BeTrue();
    }
}
