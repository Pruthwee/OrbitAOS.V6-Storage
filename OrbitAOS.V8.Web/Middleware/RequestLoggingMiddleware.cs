namespace OrbitAOS.V8.Web.Middleware;

/// <summary>
/// Request logging middleware.
/// 
/// Migration notes from legacy ASP.NET MVC 6.0:
/// - Replaces IHttpModule implementations (registered in web.config)
/// - Middleware is registered in Program.cs via app.UseMiddleware&lt;T&gt;()
/// - Uses ILogger&lt;T&gt; instead of log4net or custom logging
/// - No more HttpApplication events (BeginRequest, EndRequest, etc.)
/// 
/// Breaking changes from HTTP Modules:
/// - IHttpModule.Init() → constructor injection
/// - HttpApplication.BeginRequest → middleware before next()
/// - HttpApplication.EndRequest → middleware after next()
/// - HttpContext.Current → injected IHttpContextAccessor or method parameter
/// 
/// Registration in Program.cs:
///   app.UseMiddleware&lt;RequestLoggingMiddleware&gt;();
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;

        _logger.LogDebug("Request started: {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        try
        {
            await _next(context);
        }
        finally
        {
            var elapsed = DateTime.UtcNow - startTime;
            _logger.LogDebug("Request completed: {Method} {Path} → {StatusCode} in {ElapsedMs}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                elapsed.TotalMilliseconds);
        }
    }
}
