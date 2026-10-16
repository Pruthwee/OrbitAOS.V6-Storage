// ============================================================
// Tests/Models/ErrorViewModelBackCompatTests.cs
// Unit Tests - Models Layer (backward compatibility)
// Tests for Models/ErrorViewModel backward-compat shim
// ============================================================

using FluentAssertions;
using OrbitAOS.V6.Models;
using Xunit;

namespace OrbitAOS.V6.Tests.Models;

/// <summary>
/// Unit tests for the backward-compatibility ErrorViewModel in the Models namespace.
/// Verifies that it correctly inherits from Web.ViewModels.ErrorViewModel.
/// </summary>
public class ErrorViewModelBackCompatTests
{
    [Fact]
    public void ErrorViewModel_ShouldInheritFromWebViewModelsErrorViewModel()
    {
        // Act
        var model = new OrbitAOS.V6.Models.ErrorViewModel();

        // Assert
        model.Should().BeAssignableTo<OrbitAOS.V6.Web.ViewModels.ErrorViewModel>();
    }

    [Fact]
    public void RequestId_ShouldDefaultToNull()
    {
        // Act
        var model = new OrbitAOS.V6.Models.ErrorViewModel();

        // Assert
        model.RequestId.Should().BeNull();
    }

    [Fact]
    public void ShowRequestId_ShouldBeFalse_WhenRequestIdIsNull()
    {
        // Arrange
        var model = new OrbitAOS.V6.Models.ErrorViewModel { RequestId = null };

        // Act & Assert
        model.ShowRequestId.Should().BeFalse();
    }

    [Fact]
    public void ShowRequestId_ShouldBeTrue_WhenRequestIdIsSet()
    {
        // Arrange
        var model = new OrbitAOS.V6.Models.ErrorViewModel { RequestId = "req-123" };

        // Act & Assert
        model.ShowRequestId.Should().BeTrue();
    }

    [Fact]
    public void RequestId_ShouldBeSettable()
    {
        // Arrange
        const string requestId = "test-request-id";

        // Act
        var model = new OrbitAOS.V6.Models.ErrorViewModel { RequestId = requestId };

        // Assert
        model.RequestId.Should().Be(requestId);
    }
}
