using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.Web.Controllers;
using OrbitAOS.Web.Models;
using FluentAssertions;
using Xunit;

namespace OrbitAOS.Tests.Controllers;

/// <summary>
/// Unit tests for <see cref="HomeController"/>.
/// Tests controller actions in isolation using mocked dependencies.
/// </summary>
public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _loggerMock;
    private readonly HomeController _controller;

    /// <summary>
    /// Initializes test fixtures for HomeController tests.
    /// </summary>
    public HomeControllerTests()
    {
        _loggerMock = new Mock<ILogger<HomeController>>();
        _controller = new HomeController(_loggerMock.Object);

        // Set up HttpContext for TraceIdentifier (needed for Error action)
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    /// <summary>
    /// Tests that Index action returns a ViewResult.
    /// </summary>
    [Fact]
    public void Index_ReturnsViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    /// <summary>
    /// Tests that Privacy action returns a ViewResult.
    /// </summary>
    [Fact]
    public void Privacy_ReturnsViewResult()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    /// <summary>
    /// Tests that Error action returns a ViewResult with ErrorViewModel.
    /// </summary>
    [Fact]
    public void Error_ReturnsViewResultWithErrorViewModel()
    {
        // Act
        var result = _controller.Error();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().BeOfType<ErrorViewModel>();
    }

    /// <summary>
    /// Tests that Error action populates RequestId from HttpContext.TraceIdentifier.
    /// </summary>
    [Fact]
    public void Error_PopulatesRequestIdFromTraceIdentifier()
    {
        // Arrange
        var expectedTraceId = "test-trace-id-12345";
        _controller.ControllerContext.HttpContext.TraceIdentifier = expectedTraceId;

        // Act
        var result = _controller.Error() as ViewResult;

        // Assert
        var model = result?.Model as ErrorViewModel;
        model.Should().NotBeNull();
        model!.RequestId.Should().Be(expectedTraceId);
        model.ShowRequestId.Should().BeTrue();
    }

    /// <summary>
    /// Tests that Error action has ResponseCache attribute with NoStore = true.
    /// </summary>
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
    }

    /// <summary>
    /// Tests that ErrorViewModel.ShowRequestId returns false when RequestId is null.
    /// </summary>
    [Fact]
    public void ErrorViewModel_ShowRequestId_ReturnsFalse_WhenRequestIdIsNull()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = null };

        // Assert
        model.ShowRequestId.Should().BeFalse();
    }

    /// <summary>
    /// Tests that ErrorViewModel.ShowRequestId returns false when RequestId is empty.
    /// </summary>
    [Fact]
    public void ErrorViewModel_ShowRequestId_ReturnsFalse_WhenRequestIdIsEmpty()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = string.Empty };

        // Assert
        model.ShowRequestId.Should().BeFalse();
    }
}
