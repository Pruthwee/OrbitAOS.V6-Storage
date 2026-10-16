using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.Web.Controllers;
using OrbitAOS.Web.Models;
using Xunit;

namespace OrbitAOS.Tests.Controllers;

/// <summary>
/// Unit tests for the HomeController.
/// Tests verify that controller actions return the correct view results.
/// </summary>
public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _mockLogger;
    private readonly HomeController _controller;

    /// <summary>Initializes test fixtures for HomeController tests.</summary>
    public HomeControllerTests()
    {
        _mockLogger = new Mock<ILogger<HomeController>>();
        _controller = new HomeController(_mockLogger.Object);
    }

    /// <summary>Verifies that the Index action returns a ViewResult.</summary>
    [Fact]
    public void Index_ReturnsViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    /// <summary>Verifies that the Privacy action returns a ViewResult.</summary>
    [Fact]
    public void Privacy_ReturnsViewResult()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    /// <summary>Verifies that the Error action returns a ViewResult with an ErrorViewModel.</summary>
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

    /// <summary>Verifies that the Error action model has a non-null RequestId from HttpContext.</summary>
    [Fact]
    public void Error_ModelHasRequestId_WhenHttpContextTraceIdentifierSet()
    {
        // Arrange
        var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
        httpContext.TraceIdentifier = "test-trace-id-12345";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = _controller.Error() as ViewResult;
        var model = result?.Model as ErrorViewModel;

        // Assert
        Assert.NotNull(model);
        Assert.Equal("test-trace-id-12345", model.RequestId);
        Assert.True(model.ShowRequestId);
    }
}
