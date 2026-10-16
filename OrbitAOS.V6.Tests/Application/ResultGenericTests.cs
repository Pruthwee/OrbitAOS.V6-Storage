// ============================================================
// Tests/Application/ResultGenericTests.cs
// Unit Tests - Application Layer
// Extended tests for Result<T> generic pattern
// ============================================================

using FluentAssertions;
using OrbitAOS.V6.Application.Common.Models;
using Xunit;

namespace OrbitAOS.V6.Tests.Application;

/// <summary>
/// Extended unit tests for the generic Result&lt;T&gt; pattern.
/// </summary>
public class ResultGenericTests
{
    [Fact]
    public void GenericResult_Success_WithIntValue_ShouldReturnValue()
    {
        // Act
        var result = Result<int>.Success(42);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().Be(42);
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void GenericResult_Success_WithObjectValue_ShouldReturnValue()
    {
        // Arrange
        var obj = new { Name = "Test", Value = 100 };

        // Act
        var result = Result<object>.Success(obj);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().Be(obj);
    }

    [Fact]
    public void GenericResult_Failure_WithMultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var errors = new[] { "Error A", "Error B" };

        // Act
        var result = Result<string>.Failure(errors);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain("Error A");
        result.Errors.Should().Contain("Error B");
    }

    [Fact]
    public void GenericResult_Failure_WithSingleError_ShouldReturnError()
    {
        // Act
        var result = Result<int>.Failure("Something failed");

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be("Something failed");
    }

    [Fact]
    public void Result_Success_ShouldHaveEmptyErrors()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.Errors.Should().NotBeNull();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Result_Failure_WithEnumerable_ShouldContainAllErrors()
    {
        // Arrange
        var errors = new List<string> { "Err1", "Err2", "Err3" };

        // Act
        var result = Result.Failure(errors);

        // Assert
        result.Errors.Should().HaveCount(3);
    }

    [Fact]
    public void GenericResult_InheritsFromResult()
    {
        // Act
        var result = Result<string>.Success("value");

        // Assert
        result.Should().BeAssignableTo<Result>();
    }

    [Fact]
    public void Result_Failure_ShouldNotSucceed()
    {
        // Act
        var result = Result.Failure("error");

        // Assert
        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public void GenericResult_Success_WithNullableType_ShouldWork()
    {
        // Act
        var result = Result<string?>.Success(null);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Theory]
    [InlineData("single error")]
    [InlineData("another error message")]
    [InlineData("validation failed")]
    public void Result_Failure_WithVariousMessages_ShouldContainMessage(string errorMessage)
    {
        // Act
        var result = Result.Failure(errorMessage);

        // Assert
        result.Errors.Should().Contain(errorMessage);
        result.Succeeded.Should().BeFalse();
    }
}
