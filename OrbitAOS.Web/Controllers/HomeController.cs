using Microsoft.AspNetCore.Mvc;
using OrbitAOS.Web.Models;
using System.Diagnostics;

namespace OrbitAOS.Web.Controllers
{
    /// <summary>
    /// Home controller providing the main landing page and error handling.
    /// Migrated from ASP.NET MVC 6.0 HomeController to ASP.NET Core MVC on .NET 10.
    /// Changes:
    ///   - Inherits from Microsoft.AspNetCore.Mvc.Controller (not System.Web.Mvc.Controller)
    ///   - Uses IActionResult return types (compatible with .NET 10)
    ///   - Constructor injection via DI (replaces legacy DependencyResolver)
    ///   - ILogger injected via DI (replaces legacy log4net/NLog manual setup)
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// GET: /Home/Index - Application landing page.
        /// </summary>
        public IActionResult Index()
        {
            _logger.LogInformation("Home/Index accessed at {Time}", DateTime.UtcNow);
            return View();
        }

        /// <summary>
        /// GET: /Home/Privacy - Privacy policy page.
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// GET: /Home/Error - Error display page.
        /// ResponseCache attribute replaces legacy OutputCache with NoStore to prevent caching error pages.
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
}
