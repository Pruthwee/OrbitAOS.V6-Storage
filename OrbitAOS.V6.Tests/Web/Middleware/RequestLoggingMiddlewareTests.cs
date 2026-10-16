// ============================================================
// Tests/Web/Middleware/RequestLoggingMiddlewareTests.cs
// Unit Tests - Web Layer Middleware
// Tests for RequestLoggingMiddleware
// ============================================================

using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Web.Middleware;
using Xunit;

namespace OrbitAOS.V6.Tests.Web.Middleware;

/// <summary>
/// Unit tests for RequestLoggingMiddleware.
/// Verifies that the middleware correctly logs requests and passes them through the pipeline.
/// </summary>
public class RequestLoggingMiddlewareTests
{
    private readonly Mock<ILogger<RequestLoggingMiddleware>> _mockLogger;

    public RequestLoggingMiddlewareTests()
    {
        _mockLogger = new Mock<ILogger<RequestLoggingMiddleware>>();
    }

    [Fact]
    public async Task InvokeAsync_ShouldCallNextMiddleware()
    {
        // Arrange
        var nextCalled = false;
        RequestDelegate next = (ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new RequestLoggingMiddleware(next, _mockLogger.Object);
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/test";

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_ShouldCompleteSuccessfully_ForGetRequest()
    {
        // Arrange
        RequestDelegate next = (ctx) => Task.CompletedTask;
        var middleware = new RequestLoggingMiddleware(next, _mockLogger.Object);

        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/api/users";

        // Act
        var act = async () => await middleware.InvokeAsync(context);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task InvokeAsync_ShouldCompleteSuccessfully_ForPostRequest()
    {
        // Arrange
        RequestDelegate next = (ctx) => Task.CompletedTask;
        var middleware = new RequestLoggingMiddleware(next, _mockLogger.Object);

        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Path = "/api/users";

        // Act
        var act = async () => await middleware.InvokeAsync(context);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task InvokeAsync_ShouldRethrowException_WhenNextThrows()
    {
        // Arrange
        RequestDelegate next = (ctx) => throw new InvalidOperationException("Test exception");
        var middleware = new RequestLoggingMiddleware(next, _mockLogger.Object);

        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/error";

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => middleware.InvokeAsync(context));
    }

    [Fact]
    public void Constructor_ShouldCreateMiddleware_WithValidParameters()
    {
        // Arrange
        RequestDelegate next = (ctx) => Task.CompletedTask;

        // Act
        var middleware = new RequestLoggingMiddleware(next, _mockLogger.Object);

        // Assert
        middleware.Should().NotBeNull();
    }

    [Fact]
    public async Task InvokeAsync_ShouldHandleRootPath()
    {
        // Arrange
        RequestDelegate next = (ctx) => Task.CompletedTask;
        var middleware = new RequestLoggingMiddleware(next, _mockLogger.Object);

        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/";

        // Act
        var act = async () => await middleware.InvokeAsync(context);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task InvokeAsync_ShouldPassContextToNextMiddleware()
    {
        // Arrange
        HttpContext? capturedContext = null;
        RequestDelegate next = (ctx) =>
        {
            capturedContext = ctx;
            return Task.CompletedTask;
        };

        var middleware = new RequestLoggingMiddleware(next, _mockLogger.Object);
        var context = new DefaultHttpContext();
        context.Request.Method = "DELETE";
        context.Request.Path = "/api/items/1";

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        capturedContext.Should().BeSameAs(context);
    }
}
