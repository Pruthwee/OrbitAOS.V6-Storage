using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OrbitAOS.Web.Filters;

/// <summary>
/// Global exception filter for handling unhandled exceptions in MVC actions.
/// Replaces the legacy HandleErrorAttribute from System.Web.Mvc.
/// Registered in Program.cs via AddControllersWithViews().AddMvcOptions().
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of <see cref="GlobalExceptionFilter"/>.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="environment">The web host environment.</param>
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
            "Unhandled exception occurred. RequestPath: {Path}",
            context.HttpContext.Request.Path);

        if (!_environment.IsDevelopment())
        {
            context.Result = new RedirectToActionResult("Error", "Home", null);
            context.ExceptionHandled = true;
        }
    }
}
