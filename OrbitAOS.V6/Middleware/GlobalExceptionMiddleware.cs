// ============================================================
// Web Layer - Global Exception Handling Middleware
// Replaces legacy IHttpModule / Application_Error in Global.asax.
// ============================================================
using System.Net;
using System.Text.Json;

namespace OrbitAOS.V6.Middleware
{
    /// <summary>
    /// Middleware that catches unhandled exceptions and returns a structured
    /// JSON error response in non-development environments.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        /// <summary>
        /// Initializes a new instance of <see cref="GlobalExceptionMiddleware"/>.
        /// </summary>
        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        /// <summary>Invokes the middleware.</summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception for request {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = _env.IsDevelopment()
                ? new { message = exception.Message, detail = exception.StackTrace }
                : new { message = "An unexpected error occurred.", detail = (string?)null };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }

    /// <summary>
    /// Extension method to register <see cref="GlobalExceptionMiddleware"/>.
    /// </summary>
    public static class GlobalExceptionMiddlewareExtensions
    {
        /// <summary>Adds the global exception middleware to the pipeline.</summary>
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
            => app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
