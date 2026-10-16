namespace OrbitAOS.Web.Middleware;

/// <summary>
/// Custom request logging middleware.
/// Replaces legacy IHttpModule implementations from System.Web.
/// Demonstrates the middleware pipeline pattern in ASP.NET Core.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="RequestLoggingMiddleware"/>.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger instance.</param>
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Processes the HTTP request and logs timing information.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;

        _logger.LogDebug(
            "Request started: {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        await _next(context);

        var elapsed = DateTime.UtcNow - startTime;

        _logger.LogDebug(
            "Request completed: {Method} {Path} - Status: {StatusCode} - Duration: {ElapsedMs}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            elapsed.TotalMilliseconds);
    }
}

/// <summary>
/// Extension methods for registering <see cref="RequestLoggingMiddleware"/>.
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    /// <summary>
    /// Adds the request logging middleware to the application pipeline.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The updated application builder.</returns>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
