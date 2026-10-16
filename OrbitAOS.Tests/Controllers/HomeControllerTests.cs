using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.Web.Controllers;
using OrbitAOS.Web.Models;
using Xunit;

namespace OrbitAOS.Tests.Controllers
{
    /// <summary>
    /// Unit tests for HomeController.
    /// Tests ASP.NET Core MVC controller actions using xUnit and Moq.
    /// Demonstrates testing pattern for .NET 10 MVC controllers.
    /// </summary>
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_mockLogger.Object);
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Error_ReturnsViewResult_WithErrorViewModel()
        {
            // Arrange - Set up HttpContext for TraceIdentifier
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
            Assert.NotNull(model);
        }

        [Fact]
        public void Error_ReturnsViewResult_WithRequestId_WhenTraceIdentifierSet()
        {
            // Arrange
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            httpContext.TraceIdentifier = "test-trace-id-12345";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
            Assert.Equal("test-trace-id-12345", model.RequestId);
            Assert.True(model.ShowRequestId);
        }

        [Fact]
        public void ErrorViewModel_ShowRequestId_ReturnsFalse_WhenRequestIdIsNull()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = null };

            // Assert
            Assert.False(model.ShowRequestId);
        }

        [Fact]
        public void ErrorViewModel_ShowRequestId_ReturnsFalse_WhenRequestIdIsEmpty()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = string.Empty };

            // Assert
            Assert.False(model.ShowRequestId);
        }

        [Fact]
        public void ErrorViewModel_ShowRequestId_ReturnsTrue_WhenRequestIdIsSet()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = "some-request-id" };

            // Assert
            Assert.True(model.ShowRequestId);
        }
    }
}
