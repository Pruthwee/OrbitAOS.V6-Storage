// ============================================================
// Web/Middleware/RequestLoggingMiddleware.cs
// Clean Architecture - Web Layer
// Custom middleware replacing legacy HTTP Module (IHttpModule)
// ============================================================

namespace OrbitAOS.V6.Web.Middleware;

/// <summary>
/// Request logging middleware for ASP.NET Core .NET 10.
///
/// Migration from legacy HTTP Module (IHttpModule):
/// -------------------------------------------------------
/// LEGACY (System.Web - NOT available in .NET 10):
///   public class RequestLoggingModule : IHttpModule
///   {
///       public void Init(HttpApplication context)
///       {
///           context.BeginRequest += OnBeginRequest;
///           context.EndRequest += OnEndRequest;
///       }
///   }
///
/// .NET 10 EQUIVALENT (this class):
///   Registered in Program.cs: app.UseMiddleware&lt;RequestLoggingMiddleware&gt;();
///   Follows the ASP.NET Core middleware pipeline pattern.
/// -------------------------------------------------------
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
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        _logger.LogInformation(
            "Request started: {Method} {Path} at {StartTime}",
            requestMethod, requestPath, startTime);

        try
        {
            await _next(context);

            var elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
            _logger.LogInformation(
                "Request completed: {Method} {Path} → {StatusCode} in {Elapsed}ms",
                requestMethod, requestPath, context.Response.StatusCode, elapsed);
        }
        catch (Exception ex)
        {
            var elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
            _logger.LogError(ex,
                "Request failed: {Method} {Path} in {Elapsed}ms",
                requestMethod, requestPath, elapsed);
            throw;
        }
    }
}
