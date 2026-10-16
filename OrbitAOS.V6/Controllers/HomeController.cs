using Microsoft.AspNetCore.Mvc;
using OrbitAOS.V6.Models;
using System.Diagnostics;

namespace OrbitAOS.V6.Controllers;

/// <summary>
/// Home controller for the OrbitAOS application.
/// Upgraded from ASP.NET MVC 6.0 (.NET 6) to ASP.NET Core MVC on .NET 8.
/// Uses file-scoped namespace (C# 10+ feature).
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="HomeController"/>.
    /// Constructor injection replaces legacy DependencyResolver.
    /// </summary>
    /// <param name="logger">The logger instance injected via DI.</param>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Displays the home page.
    /// GET: /Home/Index or /
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Displays the privacy policy page.
    /// GET: /Home/Privacy
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Displays the error page.
    /// ResponseCache prevents caching of error responses.
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
