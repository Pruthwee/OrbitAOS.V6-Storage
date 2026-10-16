using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OrbitAOS.V6.Filters;

/// <summary>
/// Global exception filter for handling unhandled exceptions in MVC controller actions.
/// Replaces the legacy HandleErrorAttribute from System.Web.Mvc.
/// Registered in Program.cs via AddControllersWithViews(options => options.Filters.Add&lt;GlobalExceptionFilter&gt;()).
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of <see cref="GlobalExceptionFilter"/>.
    /// </summary>
    /// <param name="logger">The logger instance injected via DI.</param>
    /// <param name="environment">The web host environment for environment checks.</param>
    public GlobalExceptionFilter(
        ILogger<GlobalExceptionFilter> logger,
        IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    /// <inheritdoc/>
    public void OnException(ExceptionContext context)
    {
        _logger.LogError(
            context.Exception,
            "Unhandled exception in controller action. RequestPath: {Path}, Controller: {Controller}, Action: {Action}",
            context.HttpContext.Request.Path,
            context.RouteData.Values["controller"],
            context.RouteData.Values["action"]);

        // In production, redirect to the error page instead of showing exception details
        if (!_environment.IsDevelopment())
        {
            context.Result = new RedirectToActionResult("Error", "Home", null);
            context.ExceptionHandled = true;
        }
    }
}
