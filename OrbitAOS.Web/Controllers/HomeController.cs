using Microsoft.AspNetCore.Mvc;
using OrbitAOS.Web.Models;
using System.Diagnostics;

namespace OrbitAOS.Web.Controllers;

/// <summary>
/// Home controller providing the main landing pages for OrbitAOS.
/// Migrated from net6.0 to net10.0 ASP.NET Core MVC.
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Displays the application home page.
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Displays the privacy policy page.
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Displays the error page with request tracking information.
    /// ResponseCache is set to NoStore to prevent caching of error responses.
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
