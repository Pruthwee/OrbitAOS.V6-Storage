using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.Web.Controllers;
using OrbitAOS.Web.Models;

namespace OrbitAOS.Tests.Controllers;

/// <summary>
/// Unit tests for <see cref="HomeController"/>.
/// Tests controller actions in isolation using mocked ILogger.
/// Demonstrates ASP.NET Core MVC controller testing patterns for .NET 8.
/// </summary>
public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _loggerMock;
    private readonly HomeController _sut;

    public HomeControllerTests()
    {
        _loggerMock = new Mock<ILogger<HomeController>>();
        _sut = new HomeController(_loggerMock.Object);

        // Set up a minimal HttpContext so HttpContext.TraceIdentifier is accessible
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ── Index ────────────────────────────────────────────────────────────────

    [Fact]
    public void Index_ReturnsViewResult()
    {
        // Act
        var result = _sut.Index();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Index_ViewResult_HasNoModel()
    {
        // Act
        var result = _sut.Index() as ViewResult;

        // Assert
        result!.Model.Should().BeNull();
    }

    // ── Privacy ──────────────────────────────────────────────────────────────

    [Fact]
    public void Privacy_ReturnsViewResult()
    {
        // Act
        var result = _sut.Privacy();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    // ── Error ────────────────────────────────────────────────────────────────

    [Fact]
    public void Error_ReturnsViewResultWithErrorViewModel()
    {
        // Act
        var result = _sut.Error() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.Model.Should().BeOfType<ErrorViewModel>();
    }

    [Fact]
    public void Error_ErrorViewModel_HasRequestId()
    {
        // Arrange - TraceIdentifier is set on DefaultHttpContext
        _sut.ControllerContext.HttpContext.TraceIdentifier = "test-trace-id";

        // Act
        var result = _sut.Error() as ViewResult;
        var model = result!.Model as ErrorViewModel;

        // Assert
        model!.RequestId.Should().Be("test-trace-id");
        model.ShowRequestId.Should().BeTrue();
    }

    [Fact]
    public void Error_HasResponseCacheAttribute_WithNoStore()
    {
        // Arrange - verify the [ResponseCache] attribute is applied correctly
        var methodInfo = typeof(HomeController).GetMethod(nameof(HomeController.Error));
        var attribute = methodInfo!
            .GetCustomAttributes(typeof(ResponseCacheAttribute), false)
            .FirstOrDefault() as ResponseCacheAttribute;

        // Assert
        attribute.Should().NotBeNull();
        attribute!.NoStore.Should().BeTrue();
        attribute.Duration.Should().Be(0);
        attribute.Location.Should().Be(ResponseCacheLocation.None);
    }
}
