// ============================================================
// Controllers/HomeController.cs
// Clean Architecture - Web Layer
// Upgraded from net6.0 to net10.0
// Uses ILogger<T> via DI (replaces static log4net/ELMAH)
// ============================================================

using Microsoft.AspNetCore.Mvc;
using OrbitAOS.V6.Web.ViewModels;
using System.Diagnostics;

namespace OrbitAOS.V6.Controllers;

/// <summary>
/// Home controller providing the application's main entry points.
///
/// Migration changes from net6.0:
/// - Namespace updated to OrbitAOS.V6.Controllers (Clean Architecture Web layer)
/// - ILogger&lt;HomeController&gt; injected via constructor DI (unchanged from net6.0)
/// - IActionResult return types maintained (compatible with .NET 10)
/// - [ResponseCache] attribute works with Response Caching middleware in .NET 10
/// - ErrorViewModel moved to Web/ViewModels/ folder (Clean Architecture)
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Application home page.
    /// GET /Home/Index or GET /
    /// </summary>
    public IActionResult Index()
    {
        _logger.LogInformation("Home page accessed at {Time}", DateTime.UtcNow);
        return View();
    }

    /// <summary>
    /// Privacy policy page.
    /// GET /Home/Privacy
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Error page - displays request ID for diagnostics.
    /// ResponseCache: NoStore prevents caching of error responses.
    /// Replaces: CustomErrors in web.config + HandleErrorAttribute in FilterConfig.cs
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogError("Error page displayed for request {RequestId}", requestId);

        return View(new ErrorViewModel
        {
            RequestId = requestId
        });
    }
}
