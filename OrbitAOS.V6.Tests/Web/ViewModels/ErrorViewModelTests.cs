// ============================================================
// Tests/Web/ViewModels/ErrorViewModelTests.cs
// Unit Tests - Web Layer ViewModels
// Tests for ErrorViewModel
// ============================================================

using FluentAssertions;
using OrbitAOS.V6.Web.ViewModels;
using Xunit;

namespace OrbitAOS.V6.Tests.Web.ViewModels;

/// <summary>
/// Unit tests for ErrorViewModel.
/// </summary>
public class ErrorViewModelTests
{
    [Fact]
    public void RequestId_ShouldDefaultToNull()
    {
        // Act
        var model = new ErrorViewModel();

        // Assert
        model.RequestId.Should().BeNull();
    }

    [Fact]
    public void ShowRequestId_ShouldBeFalse_WhenRequestIdIsNull()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = null };

        // Act & Assert
        model.ShowRequestId.Should().BeFalse();
    }

    [Fact]
    public void ShowRequestId_ShouldBeFalse_WhenRequestIdIsEmpty()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = string.Empty };

        // Act & Assert
        model.ShowRequestId.Should().BeFalse();
    }

    [Fact]
    public void ShowRequestId_ShouldBeTrue_WhenRequestIdHasValue()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = "00-abc123-def456-00" };

        // Act & Assert
        model.ShowRequestId.Should().BeTrue();
    }

    [Fact]
    public void RequestId_ShouldBeSettable()
    {
        // Arrange
        const string requestId = "trace-id-12345";

        // Act
        var model = new ErrorViewModel { RequestId = requestId };

        // Assert
        model.RequestId.Should().Be(requestId);
    }

    [Fact]
    public void ShowRequestId_ShouldBeTrue_WhenRequestIdIsWhitespace()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = "   " };

        // Act & Assert
        // Non-empty whitespace string is not null or empty, so ShowRequestId is true
        model.ShowRequestId.Should().BeTrue();
    }
}
