// ============================================================
// Tests/Application/ResultTests.cs
// Unit Tests - Application Layer Result pattern tests
// ============================================================

using FluentAssertions;
using OrbitAOS.V6.Application.Common.Models;
using Xunit;

namespace OrbitAOS.V6.Tests.Application;

/// <summary>
/// Unit tests for the Result pattern used in the Application layer.
/// </summary>
public class ResultTests
{
    [Fact]
    public void Success_ShouldReturnSucceededResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Failure_WithSingleError_ShouldReturnFailedResult()
    {
        // Act
        var result = Result.Failure("Something went wrong");

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be("Something went wrong");
    }

    [Fact]
    public void Failure_WithMultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var errors = new[] { "Error 1", "Error 2", "Error 3" };

        // Act
        var result = Result.Failure(errors);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void GenericSuccess_ShouldReturnValueAndSucceeded()
    {
        // Act
        var result = Result<string>.Success("test-value");

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().Be("test-value");
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void GenericFailure_ShouldReturnNullValueAndErrors()
    {
        // Act
        var result = Result<string>.Failure("Operation failed");

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Errors.Should().ContainSingle();
    }
}
