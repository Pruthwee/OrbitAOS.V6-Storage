using OrbitAOS.Application.DTOs;
using OrbitAOS.Application.Interfaces;
using OrbitAOS.Domain.Common;
using Microsoft.Extensions.Logging;

namespace OrbitAOS.Application.Services
{
    /// <summary>
    /// Application service for user management operations.
    /// Delegates identity operations to the infrastructure layer via IUserService.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserService _identityUserService;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserService identityUserService, ILogger<UserService> logger)
        {
            _identityUserService = identityUserService;
            _logger = logger;
        }

        public async Task<Result<UserDto>> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving user with ID: {UserId}", userId);
            return await _identityUserService.GetUserByIdAsync(userId, cancellationToken);
        }

        public async Task<Result<UserDto>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving user with email: {Email}", email);
            return await _identityUserService.GetUserByEmailAsync(email, cancellationToken);
        }

        public async Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all users");
            return await _identityUserService.GetAllUsersAsync(cancellationToken);
        }

        public async Task<Result<UserDto>> RegisterAsync(RegisterUserDto registerDto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Registering new user with email: {Email}", registerDto.Email);
            return await _identityUserService.RegisterAsync(registerDto, cancellationToken);
        }

        public async Task<Result> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);
            return await _identityUserService.LoginAsync(loginDto, cancellationToken);
        }

        public async Task<Result> LogoutAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("User logout requested");
            return await _identityUserService.LogoutAsync(cancellationToken);
        }

        public async Task<Result> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting user with ID: {UserId}", userId);
            return await _identityUserService.DeleteUserAsync(userId, cancellationToken);
        }

        public async Task<Result> AssignRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Assigning role {Role} to user {UserId}", role, userId);
            return await _identityUserService.AssignRoleAsync(userId, role, cancellationToken);
        }

        public async Task<Result> RemoveRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Removing role {Role} from user {UserId}", role, userId);
            return await _identityUserService.RemoveRoleAsync(userId, role, cancellationToken);
        }
    }
}
