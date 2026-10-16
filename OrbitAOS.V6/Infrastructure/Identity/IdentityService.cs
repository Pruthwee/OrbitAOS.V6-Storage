// ============================================================
// Infrastructure/Identity/IdentityService.cs
// Clean Architecture - Infrastructure Layer
// ASP.NET Core Identity service for .NET 10
// Replaces: MembershipProvider, RoleProvider (legacy MVC)
// ============================================================

using Microsoft.AspNetCore.Identity;
using OrbitAOS.V6.Application.Common.Interfaces;
using OrbitAOS.V6.Application.Common.Models;
using OrbitAOS.V6.Domain.Entities;

namespace OrbitAOS.V6.Infrastructure.Identity;

/// <summary>
/// Identity service wrapping ASP.NET Core Identity for .NET 10.
/// 
/// Migration from legacy patterns:
/// - Replaces: Membership.ValidateUser() → SignInManager.PasswordSignInAsync()
/// - Replaces: Roles.IsUserInRole() → UserManager.IsInRoleAsync()
/// - Replaces: Membership.CreateUser() → UserManager.CreateAsync()
/// - Replaces: FormsAuthentication.SetAuthCookie() → SignInManager.SignInAsync()
/// - Replaces: FormsAuthentication.SignOut() → SignInManager.SignOutAsync()
/// </summary>
public class IdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<IdentityService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new user account.
    /// Replaces: Membership.CreateUser() from legacy MVC.
    /// </summary>
    public async Task<Result<string>> CreateUserAsync(
        string email,
        string password,
        string? firstName = null,
        string? lastName = null)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            _logger.LogWarning("Failed to create user {Email}: {Errors}", email, string.Join(", ", errors));
            return Result<string>.Failure(errors);
        }

        _logger.LogInformation("User {Email} created successfully", email);
        return Result<string>.Success(user.Id);
    }

    /// <summary>
    /// Authenticates a user with email and password.
    /// Replaces: FormsAuthentication.Authenticate() from legacy MVC.
    /// </summary>
    public async Task<Result> SignInAsync(string email, string password, bool rememberMe = false)
    {
        var result = await _signInManager.PasswordSignInAsync(
            email,
            password,
            rememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} signed in successfully", email);
            return Result.Success();
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning("User {Email} account is locked out", email);
            return Result.Failure("Account is locked out. Please try again later.");
        }

        if (result.RequiresTwoFactor)
        {
            return Result.Failure("Two-factor authentication is required.");
        }

        return Result.Failure("Invalid email or password.");
    }

    /// <summary>
    /// Signs out the current user.
    /// Replaces: FormsAuthentication.SignOut() from legacy MVC.
    /// </summary>
    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User signed out");
    }

    /// <summary>
    /// Assigns a role to a user.
    /// Replaces: Roles.AddUserToRole() from legacy MVC.
    /// </summary>
    public async Task<Result> AddToRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Failure($"User with ID '{userId}' not found.");

        var result = await _userManager.AddToRoleAsync(user, role);
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }

    /// <summary>
    /// Gets the user ID for a given username/email.
    /// Replaces: Membership.GetUser(username).ProviderUserKey from legacy MVC.
    /// </summary>
    public async Task<string?> GetUserIdAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user?.Id;
    }
}
