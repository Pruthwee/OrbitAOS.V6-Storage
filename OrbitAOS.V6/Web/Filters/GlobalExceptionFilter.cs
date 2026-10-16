// ============================================================
// Web/Filters/GlobalExceptionFilter.cs
// Clean Architecture - Web Layer
// Global exception filter replacing App_Start/FilterConfig.cs HandleErrorAttribute
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OrbitAOS.V6.Application.Common.Exceptions;
using OrbitAOS.V6.Web.ViewModels;
using ValidationException = OrbitAOS.V6.Application.Common.Exceptions.ValidationException;

namespace OrbitAOS.V6.Web.Middleware;

/// <summary>
/// Global exception filter for ASP.NET Core MVC on .NET 10.
///
/// Migration from legacy FilterConfig.cs:
/// -------------------------------------------------------
/// LEGACY (App_Start/FilterConfig.cs):
///   public class FilterConfig
///   {
///       public static void RegisterGlobalFilters(GlobalFilterCollection filters)
///       {
///           filters.Add(new HandleErrorAttribute());
///       }
///   }
///
/// .NET 10 EQUIVALENT (this class):
///   Registered in Program.cs:
///   builder.Services.AddControllersWithViews(options =>
///       options.Filters.Add&lt;GlobalExceptionFilter&gt;());
/// -------------------------------------------------------
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionFilter(
        ILogger<GlobalExceptionFilter> logger,
        IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception,
            "Unhandled exception in {Controller}.{Action}",
            context.RouteData.Values["controller"],
            context.RouteData.Values["action"]);

        switch (context.Exception)
        {
            case NotFoundException notFoundEx:
                context.Result = new NotFoundObjectResult(new { message = notFoundEx.Message });
                context.ExceptionHandled = true;
                break;

            case ValidationException validationEx:
                context.Result = new BadRequestObjectResult(new
                {
                    message = "Validation failed",
                    errors = validationEx.Errors
                });
                context.ExceptionHandled = true;
                break;

            case ForbiddenAccessException:
                context.Result = new ForbidResult();
                context.ExceptionHandled = true;
                break;

            default:
                if (!_environment.IsDevelopment())
                {
                    // In production, redirect to error page (replaces CustomErrors in web.config)
                    context.Result = new RedirectToActionResult("Error", "Home", null);
                    context.ExceptionHandled = true;
                }
                // In development, let the developer exception page handle it
                break;
        }
    }
}
