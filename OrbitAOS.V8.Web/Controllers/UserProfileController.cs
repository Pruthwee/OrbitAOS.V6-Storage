using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OrbitAOS.V8.Application.DTOs;
using OrbitAOS.V8.Application.Interfaces;

namespace OrbitAOS.V8.Web.Controllers;

/// <summary>
/// Controller for user profile management.
/// 
/// Migration notes from legacy ASP.NET MVC 6.0:
/// - Uses constructor injection (DI) instead of DependencyResolver
/// - Async action methods throughout (no synchronous I/O)
/// - IActionResult return types
/// - [Authorize] attribute works the same as legacy MVC
/// - No more User.Identity.GetUserId() extension — use User.FindFirst() or UserManager
/// </summary>
[Authorize]
public class UserProfileController : Controller
{
    private readonly IUserProfileService _userProfileService;
    private readonly ILogger<UserProfileController> _logger;

    public UserProfileController(
        IUserProfileService userProfileService,
        ILogger<UserProfileController> logger)
    {
        _userProfileService = userProfileService;
        _logger = logger;
    }

    /// <summary>Lists all user profiles (admin view).</summary>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var profiles = await _userProfileService.GetAllAsync(cancellationToken);
        return View(profiles);
    }

    /// <summary>Shows the current user's profile.</summary>
    public async Task<IActionResult> MyProfile(CancellationToken cancellationToken)
    {
        // Migration note: User.FindFirst(ClaimTypes.NameIdentifier)?.Value replaces
        // User.Identity.GetUserId() from Microsoft.AspNet.Identity (legacy MVC 6.0)
        var identityUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (identityUserId is null)
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        var profile = await _userProfileService.GetByIdentityUserIdAsync(identityUserId, cancellationToken);
        if (profile is null)
        {
            return NotFound();
        }

        return View(profile);
    }

    /// <summary>Shows the edit form for a user profile.</summary>
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var profile = await _userProfileService.GetByIdAsync(id, cancellationToken);
        if (profile is null)
        {
            return NotFound();
        }

        return View(profile);
    }

    /// <summary>Processes the edit form submission.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserProfileDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await _userProfileService.UpdateAsync(dto, cancellationToken);
            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction(nameof(MyProfile));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to update user profile {Id}", dto.Id);
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
    }
}
