using Microsoft.AspNetCore.Mvc;
using OrbitAOS.V6.Models;
using System.Diagnostics;

namespace OrbitAOS.V6.Controllers;

/// <summary>
/// Home controller - upgraded from ASP.NET MVC 6.0 (net6.0) to ASP.NET Core MVC on .NET 8.
/// Changes applied:
///   - File-scoped namespace (C# 10+ feature, best practice for .NET 8)
///   - IActionResult return types preserved (fully compatible with .NET 8)
///   - ILogger DI injection preserved (fully compatible with .NET 8)
///   - [ResponseCache] attribute preserved (replaces legacy [OutputCache])
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    /// <summary>Initializes a new instance of <see cref="HomeController"/>.</summary>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>Renders the application home/landing page.</summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>Renders the privacy policy page.</summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Renders the error page.
    /// [ResponseCache] with NoStore=true prevents caching of error responses.
    /// This replaces the legacy [OutputCache(NoStore = true, Duration = 0)] from MVC 6.0.
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
