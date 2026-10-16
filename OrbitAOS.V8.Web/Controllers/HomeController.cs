using Microsoft.AspNetCore.Mvc;
using OrbitAOS.V8.Web.Models;
using System.Diagnostics;

namespace OrbitAOS.V8.Web.Controllers;

/// <summary>
/// Home controller for the OrbitAOS application.
/// 
/// Migration notes from legacy ASP.NET MVC 6.0:
/// - Inherits from Microsoft.AspNetCore.Mvc.Controller (not System.Web.Mvc.Controller)
/// - Constructor injection via DI (no DependencyResolver.Current.GetService())
/// - IActionResult return type (same as legacy MVC 5/6)
/// - ILogger&lt;T&gt; instead of log4net or custom logging
/// - No more HttpContext.Current — use this.HttpContext (injected)
/// - No more Request.IsAuthenticated — use User.Identity?.IsAuthenticated
/// - No more Session["key"] — use HttpContext.Session.GetString("key")
/// 
/// Breaking changes:
/// - [HandleError] attribute removed — use exception handling middleware
/// - Html.Action() / Html.RenderAction() removed — use ViewComponents
/// - ChildActionOnly attribute removed — use ViewComponents
/// - AsyncController base class removed — all controllers support async
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Home page action.
    /// Migration note: Returns IActionResult (same as legacy MVC 5/6).
    /// </summary>
    public IActionResult Index()
    {
        _logger.LogInformation("Home/Index accessed at {Time}", DateTime.UtcNow);
        return View();
    }

    /// <summary>
    /// Privacy policy page.
    /// Migration note: Replaces legacy About/Contact actions from default MVC template.
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Error page action.
    /// Migration note: [ResponseCache] replaces [OutputCache] from System.Web.Mvc.
    /// ResponseCacheLocation.None ensures error pages are never cached.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
