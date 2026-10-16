// ============================================================
// Web Layer - HomeController
// Upgraded to .NET 8 with Clean Architecture DI.
// ============================================================
using Microsoft.AspNetCore.Mvc;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Models;
using System.Diagnostics;

namespace OrbitAOS.V6.Controllers
{
    /// <summary>
    /// Home controller — handles the default landing pages.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeService _homeService;

        /// <summary>
        /// Initializes a new instance of <see cref="HomeController"/>.
        /// </summary>
        /// <param name="logger">Logger injected via DI.</param>
        /// <param name="homeService">Home service injected via DI (Application Layer).</param>
        public HomeController(ILogger<HomeController> logger, IHomeService homeService)
        {
            _logger = logger;
            _homeService = homeService;
        }

        /// <summary>Renders the home/index page.</summary>
        public IActionResult Index()
        {
            ViewData["WelcomeMessage"] = _homeService.GetWelcomeMessage();
            return View();
        }

        /// <summary>Renders the privacy policy page.</summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Renders the error page.
        /// Uses ResponseCache to prevent caching of error responses.
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
