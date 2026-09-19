using Microsoft.AspNetCore.Mvc;
using OrbitAOS.V6.Web.Models;
using System.Diagnostics;

namespace OrbitAOS.V6.Web.Controllers;

/// <summary>
/// Home controller handling the main application pages.
/// Provides Index, Privacy, and Error action methods.
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="HomeController"/>.
    /// </summary>
    /// <param name="logger">The logger instance injected via DI.</param>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Displays the application home page.
    /// </summary>
    /// <returns>The Index view.</returns>
    public IActionResult Index()
    {
        _logger.LogInformation("Home page accessed");
        return View();
    }

    /// <summary>
    /// Displays the privacy policy page.
    /// </summary>
    /// <returns>The Privacy view.</returns>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Displays the error page. Decorated with ResponseCache to prevent caching of error responses.
    /// </summary>
    /// <returns>The Error view with request ID information.</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
