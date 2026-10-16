using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OrbitAOS.V8.Application.Interfaces;

namespace OrbitAOS.V8.Web.Filters;

/// <summary>
/// Action filter for audit logging.
/// 
/// Migration notes from legacy ASP.NET MVC 6.0:
/// - Implements IAsyncActionFilter instead of ActionFilterAttribute (System.Web.Mvc)
/// - Uses constructor injection (DI) — legacy filters used static service locator
/// - Async execution (no synchronous OnActionExecuting/OnActionExecuted)
/// - Registered in Program.cs via options.Filters.Add() or as attribute
/// 
/// Breaking changes:
/// - ActionFilterAttribute from System.Web.Mvc is NOT available in .NET 8
/// - Use IActionFilter or IAsyncActionFilter from Microsoft.AspNetCore.Mvc.Filters
/// - filterContext.HttpContext replaces filterContext.RequestContext.HttpContext
/// - filterContext.ActionDescriptor.ActionName replaces filterContext.ActionDescriptor.ActionName
/// </summary>
public class AuditActionFilter : IAsyncActionFilter
{
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<AuditActionFilter> _logger;

    public AuditActionFilter(IAuditLogService auditLogService, ILogger<AuditActionFilter> logger)
    {
        _auditLogService = auditLogService;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var actionName = context.ActionDescriptor.DisplayName ?? "Unknown";
        var userId = context.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();

        // Execute the action
        var resultContext = await next();

        // Log after execution (only for non-GET requests to avoid noise)
        if (!string.Equals(context.HttpContext.Request.Method, "GET", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                await _auditLogService.LogAsync(
                    action: actionName,
                    entityType: context.Controller.GetType().Name,
                    userId: userId,
                    ipAddress: ipAddress,
                    details: resultContext.Exception?.Message);
            }
            catch (Exception ex)
            {
                // Never let audit logging failures break the request
                _logger.LogWarning(ex, "Failed to record audit log for action {Action}", actionName);
            }
        }
    }
}
