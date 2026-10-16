using OrbitAOS.Domain.Common;
using Xunit;

namespace OrbitAOS.Tests.Domain
{
    /// <summary>
    /// Unit tests for the Result domain type.
    /// Tests the Result pattern used throughout the Clean Architecture layers.
    /// </summary>
    public class ResultTests
    {
        [Fact]
        public void Success_ReturnsSuccessResult()
        {
            // Act
            var result = Result.Success();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Null(result.Error);
        }

        [Fact]
        public void Failure_ReturnsFailureResult_WithErrorMessage()
        {
            // Arrange
            var errorMessage = "Something went wrong.";

            // Act
            var result = Result.Failure(errorMessage);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(errorMessage, result.Error);
        }

        [Fact]
        public void SuccessGeneric_ReturnsSuccessResult_WithValue()
        {
            // Arrange
            var value = "test-value";

            // Act
            var result = Result.Success(value);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public void FailureGeneric_ReturnsFailureResult_WithErrorMessage()
        {
            // Arrange
            var errorMessage = "Not found.";

            // Act
            var result = Result.Failure<string>(errorMessage);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.Error);
        }

        [Fact]
        public void FailureGeneric_ThrowsException_WhenAccessingValue()
        {
            // Arrange
            var result = Result.Failure<string>("Error occurred.");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _ = result.Value);
        }

        [Fact]
        public void SuccessGeneric_WithIntValue_ReturnsCorrectValue()
        {
            // Arrange
            var value = 42;

            // Act
            var result = Result.Success(value);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(42, result.Value);
        }

        [Fact]
        public void IsFailure_IsOppositeOfIsSuccess_ForSuccess()
        {
            // Act
            var result = Result.Success();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
        }

        [Fact]
        public void IsFailure_IsOppositeOfIsSuccess_ForFailure()
        {
            // Act
            var result = Result.Failure("error");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
        }
    }
}
