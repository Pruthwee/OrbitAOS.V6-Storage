using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.Web.Controllers;
using OrbitAOS.Web.Models;
using Xunit;

namespace OrbitAOS.Tests.Web.Controllers;

/// <summary>
/// Unit tests for the HomeController.
/// Tests controller actions in isolation using Moq for dependencies.
/// </summary>
public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _loggerMock;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _loggerMock = new Mock<ILogger<HomeController>>();
        _controller = new HomeController(_loggerMock.Object);

        // Set up a default HttpContext for the controller
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public void Index_ReturnsViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName); // Uses default view name
    }

    [Fact]
    public void Privacy_ReturnsViewResult()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName); // Uses default view name
    }

    [Fact]
    public void Error_ReturnsViewResultWithErrorViewModel()
    {
        // Act
        var result = _controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
        Assert.NotNull(model);
    }

    [Fact]
    public void Error_WhenRequestIdIsAvailable_ShowsRequestId()
    {
        // Arrange - simulate a request with a trace identifier
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.HttpContext.TraceIdentifier = "test-trace-id-12345";

        // Act
        var result = _controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
        Assert.NotNull(model.RequestId);
        Assert.True(model.ShowRequestId);
    }

    [Fact]
    public void Error_HasNoCacheResponseCacheAttribute()
    {
        // Arrange
        var methodInfo = typeof(HomeController).GetMethod(nameof(HomeController.Error));

        // Act
        var attribute = methodInfo?
            .GetCustomAttributes(typeof(ResponseCacheAttribute), false)
            .FirstOrDefault() as ResponseCacheAttribute;

        // Assert
        Assert.NotNull(attribute);
        Assert.Equal(0, attribute.Duration);
        Assert.Equal(ResponseCacheLocation.None, attribute.Location);
        Assert.True(attribute.NoStore);
    }
}
