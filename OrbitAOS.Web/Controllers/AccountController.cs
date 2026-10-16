using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitAOS.Application.DTOs;
using OrbitAOS.Application.Interfaces;

namespace OrbitAOS.Web.Controllers
{
    /// <summary>
    /// Account controller handling user authentication (login/logout/register).
    /// Replaces legacy AccountController using Forms Authentication and ASP.NET Identity 2.x.
    /// Migration changes:
    ///   - Uses ASP.NET Core Identity via IUserService (Clean Architecture pattern)
    ///   - Cookie authentication replaces FormsAuthentication.SetAuthCookie()
    ///   - IActionResult replaces ActionResult
    ///   - Async/await pattern throughout
    ///   - Anti-forgery token validation via [ValidateAntiForgeryToken]
    /// </summary>
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var result = await _userService.LoginAsync(model);

            if (result.IsSuccess)
            {
                _logger.LogInformation("User {Email} logged in successfully.", model.Email);
                return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);
            }

            ModelState.AddModelError(string.Empty, result.Error ?? "Login failed.");
            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _userService.RegisterAsync(model);

            if (result.IsSuccess)
            {
                _logger.LogInformation("New user registered with email: {Email}", model.Email);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, result.Error ?? "Registration failed.");
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
