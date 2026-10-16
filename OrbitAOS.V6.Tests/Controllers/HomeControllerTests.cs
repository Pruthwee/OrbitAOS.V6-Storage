// ============================================================
// Unit Tests - HomeController Tests
// Tests the HomeController using xUnit + Moq.
// ============================================================
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Controllers;
using OrbitAOS.V6.Models;
using Xunit;

namespace OrbitAOS.V6.Tests.Controllers
{
    /// <summary>
    /// Unit tests for <see cref="HomeController"/>.
    /// </summary>
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _loggerMock;
        private readonly Mock<IHomeService> _homeServiceMock;
        private readonly HomeController _controller;

        /// <summary>
        /// Initializes test fixtures.
        /// </summary>
        public HomeControllerTests()
        {
            _loggerMock = new Mock<ILogger<HomeController>>();
            _homeServiceMock = new Mock<IHomeService>();
            _controller = new HomeController(_loggerMock.Object, _homeServiceMock.Object);
        }

        /// <summary>
        /// Index action should return a ViewResult.
        /// </summary>
        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            _homeServiceMock.Setup(s => s.GetWelcomeMessage()).Returns("Welcome!");

            // Act
            var result = _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        /// <summary>
        /// Index action should set WelcomeMessage in ViewData.
        /// </summary>
        [Fact]
        public void Index_SetsWelcomeMessageInViewData()
        {
            // Arrange
            const string expectedMessage = "Welcome to OrbitAOS on .NET 8!";
            _homeServiceMock.Setup(s => s.GetWelcomeMessage()).Returns(expectedMessage);

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result!.ViewData["WelcomeMessage"].Should().Be(expectedMessage);
        }

        /// <summary>
        /// Privacy action should return a ViewResult.
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
        /// Error action should return a ViewResult with an ErrorViewModel.
        /// </summary>
        [Fact]
        public void Error_ReturnsViewResultWithErrorViewModel()
        {
            // Arrange - set up a fake HttpContext
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.Error() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result!.Model.Should().BeOfType<ErrorViewModel>();
        }

        /// <summary>
        /// HomeService.GetWelcomeMessage should be called exactly once during Index.
        /// </summary>
        [Fact]
        public void Index_CallsHomeServiceOnce()
        {
            // Arrange
            _homeServiceMock.Setup(s => s.GetWelcomeMessage()).Returns("Hello");

            // Act
            _controller.Index();

            // Assert
            _homeServiceMock.Verify(s => s.GetWelcomeMessage(), Times.Once);
        }
    }
}
