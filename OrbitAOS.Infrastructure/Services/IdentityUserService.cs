using Microsoft.AspNetCore.Identity;
using OrbitAOS.Application.DTOs;
using OrbitAOS.Application.Interfaces;
using OrbitAOS.Domain.Common;
using OrbitAOS.Infrastructure.Data;

namespace OrbitAOS.Infrastructure.Services
{
    /// <summary>
    /// Infrastructure implementation of IUserService using ASP.NET Core Identity on .NET 10.
    /// Replaces legacy Forms Authentication and ASP.NET Identity 2.x from MVC 6.0.
    /// </summary>
    public class IdentityUserService : IUserService
    {
        private readonly UserManager<ApplicationIdentityUser> _userManager;
        private readonly SignInManager<ApplicationIdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityUserService(
            UserManager<ApplicationIdentityUser> userManager,
            SignInManager<ApplicationIdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<Result<UserDto>> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure<UserDto>($"User with ID '{userId}' not found.");

            var roles = await _userManager.GetRolesAsync(user);
            return Result.Success(MapToDto(user, roles));
        }

        public async Task<Result<UserDto>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result.Failure<UserDto>($"User with email '{email}' not found.");

            var roles = await _userManager.GetRolesAsync(user);
            return Result.Success(MapToDto(user, roles));
        }

        public async Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            var users = _userManager.Users.ToList();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(MapToDto(user, roles));
            }

            return Result.Success<IEnumerable<UserDto>>(userDtos);
        }

        public async Task<Result<UserDto>> RegisterAsync(RegisterUserDto registerDto, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
                return Result.Failure<UserDto>("A user with this email already exists.");

            var user = new ApplicationIdentityUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return Result.Failure<UserDto>($"Registration failed: {errors}");
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Result.Success(MapToDto(user, roles));
        }

        public async Task<Result> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
        {
            var result = await _signInManager.PasswordSignInAsync(
                loginDto.Email,
                loginDto.Password,
                loginDto.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
                return Result.Success();

            if (result.IsLockedOut)
                return Result.Failure("Account is locked out. Please try again later.");

            if (result.RequiresTwoFactor)
                return Result.Failure("Two-factor authentication is required.");

            return Result.Failure("Invalid email or password.");
        }

        public async Task<Result> LogoutAsync(CancellationToken cancellationToken = default)
        {
            await _signInManager.SignOutAsync();
            return Result.Success();
        }

        public async Task<Result> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure($"User with ID '{userId}' not found.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return Result.Failure($"Delete failed: {errors}");
            }

            return Result.Success();
        }

        public async Task<Result> AssignRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure($"User with ID '{userId}' not found.");

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return Result.Failure($"Role assignment failed: {errors}");
            }

            return Result.Success();
        }

        public async Task<Result> RemoveRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure($"User with ID '{userId}' not found.");

            var result = await _userManager.RemoveFromRoleAsync(user, role);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return Result.Failure($"Role removal failed: {errors}");
            }

            return Result.Success();
        }

        private static UserDto MapToDto(ApplicationIdentityUser user, IList<string> roles)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Roles = roles
            };
        }
    }
}
