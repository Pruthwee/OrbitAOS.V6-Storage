using Microsoft.AspNetCore.Mvc;
using OrbitAOS.Web.Models;
using System.Diagnostics;

namespace OrbitAOS.Web.Controllers;

/// <summary>
/// Home controller handling the main application pages.
/// Migrated from ASP.NET MVC 6.0 to ASP.NET Core MVC on .NET 8.
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="HomeController"/>.
    /// Uses constructor injection (replaces DependencyResolver from legacy MVC).
    /// </summary>
    /// <param name="logger">The logger instance injected via DI.</param>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Displays the application home page.
    /// GET: /Home/Index or /
    /// </summary>
    /// <returns>The home page view.</returns>
    public IActionResult Index()
    {
        _logger.LogInformation("Home page accessed at {Time}", DateTime.UtcNow);
        return View();
    }

    /// <summary>
    /// Displays the privacy policy page.
    /// GET: /Home/Privacy
    /// </summary>
    /// <returns>The privacy policy view.</returns>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Displays the error page.
    /// Uses ResponseCache to prevent caching of error responses.
    /// Replaces legacy HandleError filter behavior.
    /// </summary>
    /// <returns>The error view with request ID information.</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
