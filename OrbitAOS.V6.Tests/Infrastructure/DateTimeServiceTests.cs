// ============================================================
// Tests/Infrastructure/DateTimeServiceTests.cs
// Unit Tests - Infrastructure Services
// Tests for DateTimeService
// ============================================================

using FluentAssertions;
using OrbitAOS.V6.Infrastructure.Services;
using Xunit;

namespace OrbitAOS.V6.Tests.Infrastructure;

/// <summary>
/// Unit tests for DateTimeService.
/// Verifies that the service correctly wraps DateTime operations.
/// </summary>
public class DateTimeServiceTests
{
    private readonly DateTimeService _service;

    public DateTimeServiceTests()
    {
        _service = new DateTimeService();
    }

    [Fact]
    public void UtcNow_ShouldReturnCurrentUtcTime()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var result = _service.UtcNow;

        // Assert
        result.Should().BeAfter(before);
        result.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void UtcNow_ShouldReturnUtcKind()
    {
        // Act
        var result = _service.UtcNow;

        // Assert
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void Now_ShouldReturnCurrentLocalTime()
    {
        // Arrange
        var before = DateTime.Now.AddSeconds(-1);

        // Act
        var result = _service.Now;

        // Assert
        result.Should().BeAfter(before);
        result.Should().BeBefore(DateTime.Now.AddSeconds(1));
    }

    [Fact]
    public void Today_ShouldReturnCurrentDate()
    {
        // Arrange
        var expectedDate = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var result = _service.Today;

        // Assert
        result.Should().Be(expectedDate);
    }

    [Fact]
    public void Today_ShouldReturnDateOnlyWithoutTime()
    {
        // Act
        var result = _service.Today;

        // Assert
        result.Should().NotBe(default(DateOnly));
    }

    [Fact]
    public void UtcNow_CalledTwice_ShouldReturnIncreasingOrEqualTimes()
    {
        // Act
        var first = _service.UtcNow;
        var second = _service.UtcNow;

        // Assert
        second.Should().BeOnOrAfter(first);
    }

    [Fact]
    public void DateTimeService_ShouldImplementIDateTimeService()
    {
        // Assert
        _service.Should().BeAssignableTo<OrbitAOS.V6.Application.Common.Interfaces.IDateTimeService>();
    }
}
