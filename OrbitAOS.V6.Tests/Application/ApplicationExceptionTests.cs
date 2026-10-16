// ============================================================
// Tests/Application/ApplicationExceptionTests.cs
// Unit Tests - Application Layer Exception Tests
// Tests for NotFoundException, ValidationException, ForbiddenAccessException
// ============================================================

using FluentAssertions;
using FluentValidation.Results;
using OrbitAOS.V6.Application.Common.Exceptions;
using Xunit;

namespace OrbitAOS.V6.Tests.Application;

/// <summary>
/// Unit tests for custom application exceptions.
/// Covers NotFoundException, ValidationException, and ForbiddenAccessException.
/// </summary>
public class NotFoundExceptionTests
{
    [Fact]
    public void Constructor_Default_ShouldCreateException()
    {
        // Act
        var ex = new NotFoundException();

        // Assert
        ex.Should().NotBeNull();
        ex.Should().BeOfType<NotFoundException>();
    }

    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        // Arrange
        const string message = "Entity not found";

        // Act
        var ex = new NotFoundException(message);

        // Assert
        ex.Message.Should().Be(message);
    }

    [Fact]
    public void Constructor_WithNameAndKey_ShouldFormatMessage()
    {
        // Arrange
        const string name = "User";
        const int key = 42;

        // Act
        var ex = new NotFoundException(name, key);

        // Assert
        ex.Message.Should().Contain("User");
        ex.Message.Should().Contain("42");
        ex.Message.Should().Contain("not found");
    }

    [Fact]
    public void Constructor_WithNameAndStringKey_ShouldFormatMessage()
    {
        // Arrange
        const string name = "Product";
        const string key = "abc-123";

        // Act
        var ex = new NotFoundException(name, key);

        // Assert
        ex.Message.Should().Contain("Product");
        ex.Message.Should().Contain("abc-123");
    }

    [Fact]
    public void NotFoundException_ShouldInheritFromException()
    {
        // Act
        var ex = new NotFoundException("test");

        // Assert
        ex.Should().BeAssignableTo<Exception>();
    }
}

public class ValidationExceptionTests
{
    [Fact]
    public void Constructor_Default_ShouldSetDefaultMessage()
    {
        // Act
        var ex = new OrbitAOS.V6.Application.Common.Exceptions.ValidationException();

        // Assert
        ex.Message.Should().Contain("validation");
        ex.Errors.Should().NotBeNull();
        ex.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithFailures_ShouldGroupErrorsByProperty()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Email", "Email is required"),
            new ValidationFailure("Email", "Email is invalid"),
            new ValidationFailure("Password", "Password is too short")
        };

        // Act
        var ex = new OrbitAOS.V6.Application.Common.Exceptions.ValidationException(failures);

        // Assert
        ex.Errors.Should().ContainKey("Email");
        ex.Errors["Email"].Should().HaveCount(2);
        ex.Errors.Should().ContainKey("Password");
        ex.Errors["Password"].Should().HaveCount(1);
    }

    [Fact]
    public void Constructor_WithEmptyFailures_ShouldHaveEmptyErrors()
    {
        // Arrange
        var failures = new List<ValidationFailure>();

        // Act
        var ex = new OrbitAOS.V6.Application.Common.Exceptions.ValidationException(failures);

        // Assert
        ex.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Errors_ShouldContainCorrectErrorMessages()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name cannot be empty"),
            new ValidationFailure("Age", "Age must be positive")
        };

        // Act
        var ex = new OrbitAOS.V6.Application.Common.Exceptions.ValidationException(failures);

        // Assert
        ex.Errors["Name"].Should().Contain("Name cannot be empty");
        ex.Errors["Age"].Should().Contain("Age must be positive");
    }

    [Fact]
    public void ValidationException_ShouldInheritFromException()
    {
        // Act
        var ex = new OrbitAOS.V6.Application.Common.Exceptions.ValidationException();

        // Assert
        ex.Should().BeAssignableTo<Exception>();
    }
}

public class ForbiddenAccessExceptionTests
{
    [Fact]
    public void Constructor_ShouldSetDefaultMessage()
    {
        // Act
        var ex = new ForbiddenAccessException();

        // Assert
        ex.Message.Should().Contain("forbidden");
    }

    [Fact]
    public void ForbiddenAccessException_ShouldInheritFromException()
    {
        // Act
        var ex = new ForbiddenAccessException();

        // Assert
        ex.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void ForbiddenAccessException_CanBeThrownAndCaught()
    {
        // Act & Assert
        Action action = () => throw new ForbiddenAccessException();
        action.Should().Throw<ForbiddenAccessException>()
            .WithMessage("*forbidden*");
    }
}
