// ============================================================
// Unit Tests - HomeService Tests
// Tests the HomeService using xUnit + Moq.
// ============================================================
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Application.Services;
using Xunit;

namespace OrbitAOS.V6.Tests.Services
{
    /// <summary>
    /// Unit tests for <see cref="HomeService"/>.
    /// </summary>
    public class HomeServiceTests
    {
        private readonly Mock<ILogger<HomeService>> _loggerMock;
        private readonly HomeService _service;

        /// <summary>
        /// Initializes test fixtures.
        /// </summary>
        public HomeServiceTests()
        {
            _loggerMock = new Mock<ILogger<HomeService>>();
            _service = new HomeService(_loggerMock.Object);
        }

        /// <summary>
        /// GetWelcomeMessage should return a non-empty string.
        /// </summary>
        [Fact]
        public void GetWelcomeMessage_ReturnsNonEmptyString()
        {
            // Act
            var result = _service.GetWelcomeMessage();

            // Assert
            result.Should().NotBeNullOrWhiteSpace();
        }

        /// <summary>
        /// GetWelcomeMessage should return the expected message.
        /// </summary>
        [Fact]
        public void GetWelcomeMessage_ReturnsExpectedMessage()
        {
            // Act
            var result = _service.GetWelcomeMessage();

            // Assert
            result.Should().Contain("OrbitAOS");
        }
    }
}
