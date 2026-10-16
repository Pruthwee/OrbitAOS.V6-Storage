// ============================================================
// Tests/Controllers/HomeControllerTests.cs
// Unit Tests - xUnit + Moq for .NET 10
// Tests for HomeController (Clean Architecture Web layer)
// ============================================================

using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Controllers;
using OrbitAOS.V6.Web.ViewModels;
using System.Diagnostics;
using Xunit;

namespace OrbitAOS.V6.Tests.Controllers;

/// <summary>
/// Unit tests for HomeController.
/// Demonstrates testing ASP.NET Core MVC controllers on .NET 10
/// using xUnit, Moq, and FluentAssertions.
///
/// Migration note: These tests replace legacy MVC test patterns:
/// - No more System.Web.Mvc.Controller base class
/// - No more HttpContext.Current static access
/// - ILogger injected via Moq (replaces static log4net)
/// - IActionResult assertions replace ActionResult checks
/// </summary>
public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _mockLogger;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _mockLogger = new Mock<ILogger<HomeController>>();
        _controller = new HomeController(_mockLogger.Object);

        // Set up HttpContext for controller (replaces HttpContext.Current in legacy tests)
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
    public void Index_ReturnsViewWithNoModel()
    {
        // Act
        var result = _controller.Index() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.Model.Should().BeNull();
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
        // Arrange - Set up TraceIdentifier on HttpContext
        _controller.ControllerContext.HttpContext.TraceIdentifier = "test-trace-id-12345";

        // Act
        var result = _controller.Error() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.Model.Should().BeOfType<ErrorViewModel>();

        var model = result.Model as ErrorViewModel;
        model.Should().NotBeNull();
        model!.RequestId.Should().Be("test-trace-id-12345");
        model.ShowRequestId.Should().BeTrue();
    }

    [Fact]
    public void Error_ShowRequestId_IsFalse_WhenRequestIdIsEmpty()
    {
        // Arrange
        _controller.ControllerContext.HttpContext.TraceIdentifier = string.Empty;

        // Act
        var result = _controller.Error() as ViewResult;
        var model = result?.Model as ErrorViewModel;

        // Assert
        model.Should().NotBeNull();
        model!.ShowRequestId.Should().BeFalse();
    }

    [Fact]
    public void Error_HasResponseCacheAttribute_WithNoStore()
    {
        // Arrange - Verify the ResponseCache attribute is applied correctly
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
