using OrbitAOS.Application.DTOs;
using OrbitAOS.Application.Interfaces;
using OrbitAOS.Application.Services;
using OrbitAOS.Domain.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace OrbitAOS.Tests.Services
{
    /// <summary>
    /// Unit tests for the Application layer UserService.
    /// Tests service delegation and logging behavior.
    /// </summary>
    public class UserServiceTests
    {
        private readonly Mock<IUserService> _mockIdentityUserService;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockIdentityUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _userService = new UserService(_mockIdentityUserService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var userId = "user-123";
            var expectedUser = new UserDto { Id = userId, Email = "test@example.com" };
            _mockIdentityUserService
                .Setup(s => s.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(expectedUser));

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(userId, result.Value.Id);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsFailure_WhenUserNotFound()
        {
            // Arrange
            var userId = "nonexistent-user";
            _mockIdentityUserService
                .Setup(s => s.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<UserDto>($"User with ID '{userId}' not found."));

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(userId, result.Error);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsSuccess_WhenRegistrationSucceeds()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Email = "newuser@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };
            var expectedUser = new UserDto { Id = "new-user-id", Email = registerDto.Email };
            _mockIdentityUserService
                .Setup(s => s.RegisterAsync(registerDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(expectedUser));

            // Act
            var result = await _userService.RegisterAsync(registerDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(registerDto.Email, result.Value.Email);
        }

        [Fact]
        public async Task LoginAsync_ReturnsSuccess_WhenCredentialsAreValid()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "user@example.com", Password = "Password123!" };
            _mockIdentityUserService
                .Setup(s => s.LoginAsync(loginDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());

            // Act
            var result = await _userService.LoginAsync(loginDto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task LoginAsync_ReturnsFailure_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "user@example.com", Password = "WrongPassword" };
            _mockIdentityUserService
                .Setup(s => s.LoginAsync(loginDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure("Invalid email or password."));

            // Act
            var result = await _userService.LoginAsync(loginDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid email or password.", result.Error);
        }

        [Fact]
        public async Task LogoutAsync_ReturnsSuccess()
        {
            // Arrange
            _mockIdentityUserService
                .Setup(s => s.LogoutAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());

            // Act
            var result = await _userService.LogoutAsync();

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<UserDto>
            {
                new UserDto { Id = "1", Email = "user1@example.com" },
                new UserDto { Id = "2", Email = "user2@example.com" }
            };
            _mockIdentityUserService
                .Setup(s => s.GetAllUsersAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success<IEnumerable<UserDto>>(users));

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task DeleteUserAsync_ReturnsSuccess_WhenUserExists()
        {
            // Arrange
            var userId = "user-to-delete";
            _mockIdentityUserService
                .Setup(s => s.DeleteUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task AssignRoleAsync_ReturnsSuccess_WhenRoleAssigned()
        {
            // Arrange
            var userId = "user-123";
            var role = "Admin";
            _mockIdentityUserService
                .Setup(s => s.AssignRoleAsync(userId, role, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());

            // Act
            var result = await _userService.AssignRoleAsync(userId, role);

            // Assert
            Assert.True(result.IsSuccess);
        }
    }
}
