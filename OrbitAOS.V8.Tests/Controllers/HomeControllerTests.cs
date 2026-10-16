using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V8.Web.Controllers;
using OrbitAOS.V8.Web.Models;
using System.Diagnostics;
using Xunit;

namespace OrbitAOS.V8.Tests.Controllers;

/// <summary>
/// Unit tests for HomeController.
/// 
/// Migration notes from legacy ASP.NET MVC 6.0 testing:
/// - No more System.Web.Mvc.ControllerContext — use ControllerContext from Microsoft.AspNetCore.Mvc
/// - No more HttpContextBase/HttpRequestBase mocks — use DefaultHttpContext
/// - IActionResult instead of ActionResult for return type assertions
/// - Constructor injection makes controllers easily testable without IoC container
/// </summary>
public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _loggerMock;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _loggerMock = new Mock<ILogger<HomeController>>();
        _controller = new HomeController(_loggerMock.Object);

        // Set up HttpContext for TraceIdentifier (used in Error action)
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
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Privacy_ReturnsViewResult()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Error_ReturnsViewResult_WithErrorViewModel()
    {
        // Act
        var result = _controller.Error();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().BeOfType<ErrorViewModel>();
    }

    [Fact]
    public void Error_ErrorViewModel_HasRequestId_WhenActivityIsNull()
    {
        // Arrange — ensure no ambient Activity
        Activity.Current = null;
        _controller.ControllerContext.HttpContext.TraceIdentifier = "test-trace-id";

        // Act
        var result = _controller.Error() as ViewResult;
        var model = result?.Model as ErrorViewModel;

        // Assert
        model.Should().NotBeNull();
        model!.RequestId.Should().Be("test-trace-id");
        model.ShowRequestId.Should().BeTrue();
    }

    [Fact]
    public void Error_HasResponseCacheAttribute_WithNoStore()
    {
        // Arrange
        var methodInfo = typeof(HomeController).GetMethod(nameof(HomeController.Error));

        // Act
        var attribute = methodInfo?
            .GetCustomAttributes(typeof(ResponseCacheAttribute), false)
            .FirstOrDefault() as ResponseCacheAttribute;

        // Assert
        attribute.Should().NotBeNull();
        attribute!.NoStore.Should().BeTrue();
        attribute.Duration.Should().Be(0);
        attribute.Location.Should().Be(ResponseCacheLocation.None);
    }
}
