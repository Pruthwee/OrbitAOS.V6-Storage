using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Application.Interfaces;

namespace OrbitAOS.V6.Web.Controllers;

/// <summary>
/// MVC controller for managing user profiles.
/// Provides CRUD operations for user profile management.
/// </summary>
[Authorize]
public class UserProfileController : Controller
{
    private readonly IUserProfileService _userProfileService;
    private readonly ILogger<UserProfileController> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="UserProfileController"/>.
    /// </summary>
    /// <param name="userProfileService">The user profile service injected via DI.</param>
    /// <param name="logger">The logger instance injected via DI.</param>
    public UserProfileController(
        IUserProfileService userProfileService,
        ILogger<UserProfileController> logger)
    {
        _userProfileService = userProfileService ?? throw new ArgumentNullException(nameof(userProfileService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Displays a list of all user profiles.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The Index view with all user profiles.</returns>
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all user profiles for listing");
        var profiles = await _userProfileService.GetAllAsync(cancellationToken);
        return View(profiles);
    }

    /// <summary>
    /// Displays the details of a specific user profile.
    /// </summary>
    /// <param name="id">The user profile identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The Details view or NotFound if profile does not exist.</returns>
    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving details for user profile ID {Id}", id);
        var profile = await _userProfileService.GetByIdAsync(id, cancellationToken);
        if (profile is null)
        {
            _logger.LogWarning("User profile with ID {Id} not found", id);
            return NotFound();
        }
        return View(profile);
    }

    /// <summary>
    /// Displays the form to create a new user profile.
    /// </summary>
    /// <returns>The Create view.</returns>
    [HttpGet]
    public IActionResult Create()
    {
        return View(new UserProfileDto());
    }

    /// <summary>
    /// Handles the form submission to create a new user profile.
    /// </summary>
    /// <param name="dto">The user profile data from the form.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Redirects to Index on success; returns Create view with errors on failure.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserProfileDto dto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            _logger.LogInformation("Creating new user profile for {Email}", dto.Email);
            await _userProfileService.CreateAsync(dto, cancellationToken);
            TempData["SuccessMessage"] = "User profile created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user profile for {Email}", dto.Email);
            ModelState.AddModelError(string.Empty, "An error occurred while creating the user profile.");
            return View(dto);
        }
    }

    /// <summary>
    /// Displays the form to edit an existing user profile.
    /// </summary>
    /// <param name="id">The user profile identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The Edit view or NotFound if profile does not exist.</returns>
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Loading edit form for user profile ID {Id}", id);
        var profile = await _userProfileService.GetByIdAsync(id, cancellationToken);
        if (profile is null)
        {
            _logger.LogWarning("User profile with ID {Id} not found for editing", id);
            return NotFound();
        }
        return View(profile);
    }

    /// <summary>
    /// Handles the form submission to update an existing user profile.
    /// </summary>
    /// <param name="id">The user profile identifier from the route.</param>
    /// <param name="dto">The updated user profile data from the form.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Redirects to Index on success; returns Edit view with errors on failure.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserProfileDto dto, CancellationToken cancellationToken = default)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            _logger.LogInformation("Updating user profile ID {Id}", id);
            await _userProfileService.UpdateAsync(dto, cancellationToken);
            TempData["SuccessMessage"] = "User profile updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User profile with ID {Id} not found for update", id);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile ID {Id}", id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the user profile.");
            return View(dto);
        }
    }

    /// <summary>
    /// Displays the confirmation page to delete a user profile.
    /// </summary>
    /// <param name="id">The user profile identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The Delete confirmation view or NotFound if profile does not exist.</returns>
    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Loading delete confirmation for user profile ID {Id}", id);
        var profile = await _userProfileService.GetByIdAsync(id, cancellationToken);
        if (profile is null)
        {
            _logger.LogWarning("User profile with ID {Id} not found for deletion", id);
            return NotFound();
        }
        return View(profile);
    }

    /// <summary>
    /// Handles the confirmed deletion of a user profile.
    /// </summary>
    /// <param name="id">The user profile identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Redirects to Index on success.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user profile ID {Id}", id);
            await _userProfileService.DeleteAsync(id, cancellationToken);
            TempData["SuccessMessage"] = "User profile deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User profile with ID {Id} not found for deletion", id);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user profile ID {Id}", id);
            TempData["ErrorMessage"] = "An error occurred while deleting the user profile.";
            return RedirectToAction(nameof(Index));
        }
    }
}
