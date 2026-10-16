// ============================================================
// Web Layer - Action Logging Filter
// Replaces legacy FilterConfig.cs / IActionFilter in App_Start.
// Registered globally in Program.cs via AddControllersWithViews().
// ============================================================
using Microsoft.AspNetCore.Mvc.Filters;

namespace OrbitAOS.V6.Filters
{
    /// <summary>
    /// Global action filter that logs the start and end of every controller action.
    /// Replaces the legacy <c>FilterConfig.RegisterGlobalFilters</c> pattern.
    /// </summary>
    public class ActionLoggingFilter : IActionFilter
    {
        private readonly ILogger<ActionLoggingFilter> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="ActionLoggingFilter"/>.
        /// </summary>
        /// <param name="logger">Logger injected via DI.</param>
        public ActionLoggingFilter(ILogger<ActionLoggingFilter> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation(
                "Executing action {Controller}.{Action}",
                context.RouteData.Values["controller"],
                context.RouteData.Values["action"]);
        }

        /// <inheritdoc />
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation(
                "Executed action {Controller}.{Action}",
                context.RouteData.Values["controller"],
                context.RouteData.Values["action"]);
        }
    }
}
