using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Web.Controllers;
using OrbitAOS.V6.Web.Models;
using Xunit;

namespace OrbitAOS.V6.Tests.Controllers;

/// <summary>
/// Unit tests for <see cref="HomeController"/>.
/// Tests each action method in isolation using mocked dependencies.
/// </summary>
public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _loggerMock;
    private readonly HomeController _controller;

    /// <summary>
    /// Initializes test fixtures: creates mock logger and controller instance.
    /// </summary>
    public HomeControllerTests()
    {
        _loggerMock = new Mock<ILogger<HomeController>>();
        _controller = new HomeController(_loggerMock.Object);
    }

    /// <summary>
    /// Verifies that Index() returns a ViewResult (not null, not redirect).
    /// </summary>
    [Fact]
    public void Index_ReturnsViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    /// <summary>
    /// Verifies that Privacy() returns a ViewResult.
    /// </summary>
    [Fact]
    public void Privacy_ReturnsViewResult()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    /// <summary>
    /// Verifies that Error() returns a ViewResult with an ErrorViewModel.
    /// </summary>
    [Fact]
    public void Error_ReturnsViewResultWithErrorViewModel()
    {
        // Arrange - set up HttpContext for TraceIdentifier
        var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = _controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<ErrorViewModel>(viewResult.Model);
    }

    /// <summary>
    /// Verifies that Error() populates RequestId from HttpContext.TraceIdentifier.
    /// </summary>
    [Fact]
    public void Error_SetsRequestIdFromTraceIdentifier()
    {
        // Arrange
        var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
        httpContext.TraceIdentifier = "test-trace-id-123";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = _controller.Error() as ViewResult;
        var model = result?.Model as ErrorViewModel;

        // Assert
        Assert.NotNull(model);
        Assert.Equal("test-trace-id-123", model.RequestId);
        Assert.True(model.ShowRequestId);
    }

    /// <summary>
    /// Verifies that HomeController constructor throws when logger is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new HomeController(null!));
    }
}
