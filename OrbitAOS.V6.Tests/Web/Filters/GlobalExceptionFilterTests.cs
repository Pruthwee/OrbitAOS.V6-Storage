// ============================================================
// Tests/Web/Filters/GlobalExceptionFilterTests.cs
// Unit Tests - Web Layer Filters
// Tests for GlobalExceptionFilter
// ============================================================

using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Application.Common.Exceptions;
using OrbitAOS.V6.Web.Middleware;
using Xunit;
using ValidationException = OrbitAOS.V6.Application.Common.Exceptions.ValidationException;

namespace OrbitAOS.V6.Tests.Web.Filters;

/// <summary>
/// Unit tests for GlobalExceptionFilter.
/// Verifies that exceptions are handled correctly and appropriate results are returned.
/// </summary>
public class GlobalExceptionFilterTests
{
    private readonly Mock<ILogger<GlobalExceptionFilter>> _mockLogger;
    private readonly Mock<IWebHostEnvironment> _mockEnvironment;
    private readonly GlobalExceptionFilter _filter;

    public GlobalExceptionFilterTests()
    {
        _mockLogger = new Mock<ILogger<GlobalExceptionFilter>>();
        _mockEnvironment = new Mock<IWebHostEnvironment>();
        _filter = new GlobalExceptionFilter(_mockLogger.Object, _mockEnvironment.Object);
    }

    private ExceptionContext CreateExceptionContext(Exception exception)
    {
        var httpContext = new DefaultHttpContext();
        var routeData = new RouteData();
        routeData.Values["controller"] = "Test";
        routeData.Values["action"] = "Index";

        var actionDescriptor = new ActionDescriptor();
        var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);

        return new ExceptionContext(actionContext, new List<IFilterMetadata>())
        {
            Exception = exception
        };
    }

    [Fact]
    public void OnException_WithNotFoundException_ShouldReturnNotFoundResult()
    {
        // Arrange
        var exception = new NotFoundException("User", 42);
        var context = CreateExceptionContext(exception);

        // Act
        _filter.OnException(context);

        // Assert
        context.Result.Should().BeOfType<NotFoundObjectResult>();
        context.ExceptionHandled.Should().BeTrue();
    }

    [Fact]
    public void OnException_WithValidationException_ShouldReturnBadRequestResult()
    {
        // Arrange
        var exception = new ValidationException();
        var context = CreateExceptionContext(exception);

        // Act
        _filter.OnException(context);

        // Assert
        context.Result.Should().BeOfType<BadRequestObjectResult>();
        context.ExceptionHandled.Should().BeTrue();
    }

    [Fact]
    public void OnException_WithForbiddenAccessException_ShouldReturnForbidResult()
    {
        // Arrange
        var exception = new ForbiddenAccessException();
        var context = CreateExceptionContext(exception);

        // Act
        _filter.OnException(context);

        // Assert
        context.Result.Should().BeOfType<ForbidResult>();
        context.ExceptionHandled.Should().BeTrue();
    }

    [Fact]
    public void OnException_WithGenericException_InProduction_ShouldRedirectToError()
    {
        // Arrange
        var prodEnvironment = new Mock<IWebHostEnvironment>();
        prodEnvironment.Setup(e => e.EnvironmentName).Returns("Production");

        var prodFilter = new GlobalExceptionFilter(_mockLogger.Object, prodEnvironment.Object);
        var exception = new Exception("Unexpected error");
        var context = CreateExceptionContext(exception);

        // Act
        prodFilter.OnException(context);

        // Assert - In production, should redirect to error page
        context.Result.Should().BeOfType<RedirectToActionResult>();
        context.ExceptionHandled.Should().BeTrue();
    }

    [Fact]
    public void OnException_WithGenericException_InDevelopment_ShouldNotHandleException()
    {
        // Arrange
        var devEnvironment = new Mock<IWebHostEnvironment>();
        devEnvironment.Setup(e => e.EnvironmentName).Returns("Development");

        var devFilter = new GlobalExceptionFilter(_mockLogger.Object, devEnvironment.Object);
        var exception = new Exception("Dev error");
        var context = CreateExceptionContext(exception);

        // Act
        devFilter.OnException(context);

        // Assert - In development, exception should NOT be handled (let developer exception page handle it)
        context.ExceptionHandled.Should().BeFalse();
    }

    [Fact]
    public void OnException_WithNotFoundException_ShouldIncludeMessageInResult()
    {
        // Arrange
        var exception = new NotFoundException("Product", 99);
        var context = CreateExceptionContext(exception);

        // Act
        _filter.OnException(context);

        // Assert
        var result = context.Result as NotFoundObjectResult;
        result.Should().NotBeNull();
        result!.Value.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_ShouldCreateFilter_WithValidParameters()
    {
        // Act
        var filter = new GlobalExceptionFilter(_mockLogger.Object, _mockEnvironment.Object);

        // Assert
        filter.Should().NotBeNull();
    }
}
