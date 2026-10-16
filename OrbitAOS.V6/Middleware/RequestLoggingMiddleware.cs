namespace OrbitAOS.V6.Middleware;

/// <summary>
/// Custom request logging middleware for ASP.NET Core.
/// Replaces legacy IHttpModule implementations from System.Web.
/// Logs request method, path, status code, and duration for each HTTP request.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="RequestLoggingMiddleware"/>.
    /// </summary>
    /// <param name="next">The next middleware delegate in the pipeline.</param>
    /// <param name="logger">The logger instance injected via DI.</param>
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Processes the HTTP request, logs timing information, and invokes the next middleware.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
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
/// Extension methods for registering <see cref="RequestLoggingMiddleware"/> in the pipeline.
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    /// <summary>
    /// Adds the <see cref="RequestLoggingMiddleware"/> to the application's request pipeline.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The updated application builder.</returns>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
