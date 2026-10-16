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

    /// <summary>Initializes a new instance of HomeController with dependency injection.</summary>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>Displays the application home page.</summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>Displays the privacy policy page.</summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>Displays the error page with request tracking information.</summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
