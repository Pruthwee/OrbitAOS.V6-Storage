using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.Application.DTOs;
using OrbitAOS.Application.Interfaces;
using OrbitAOS.Domain.Common;
using OrbitAOS.Web.Controllers;
using Xunit;

namespace OrbitAOS.Tests.Controllers
{
    /// <summary>
    /// Unit tests for AccountController.
    /// Tests authentication flows using xUnit and Moq.
    /// Demonstrates testing ASP.NET Core Identity integration in .NET 10.
    /// </summary>
    public class AccountControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<AccountController>> _mockLogger;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<AccountController>>();
            _controller = new AccountController(_mockUserService.Object, _mockLogger.Object);

            // Set up default HttpContext
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public void Login_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.Login();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Login_Get_SetsReturnUrlInViewData()
        {
            // Arrange
            var returnUrl = "/Home/Index";

            // Act
            var result = _controller.Login(returnUrl) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(returnUrl, result.ViewData["ReturnUrl"]);
        }

        [Fact]
        public async Task Login_Post_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            var model = new LoginDto { Email = "", Password = "" };
            _controller.ModelState.AddModelError("Email", "Email is required.");

            // Act
            var result = await _controller.Login(model);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Login_Post_RedirectsToHome_WhenLoginSucceeds()
        {
            // Arrange
            var model = new LoginDto { Email = "test@example.com", Password = "Password123!" };
            _mockUserService
                .Setup(s => s.LoginAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());

            // Set up Url helper
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(u => u.Action(It.IsAny<Microsoft.AspNetCore.Mvc.Routing.UrlActionContext>()))
                     .Returns("/Home/Index");
            _controller.Url = urlHelper.Object;

            // Act
            var result = await _controller.Login(model);

            // Assert
            var redirectResult = Assert.IsType<LocalRedirectResult>(result);
            Assert.NotNull(redirectResult.Url);
        }

        [Fact]
        public async Task Login_Post_ReturnsViewWithError_WhenLoginFails()
        {
            // Arrange
            var model = new LoginDto { Email = "test@example.com", Password = "WrongPassword" };
            _mockUserService
                .Setup(s => s.LoginAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure("Invalid email or password."));

            // Act
            var result = await _controller.Login(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public void Register_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.Register();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Register_Post_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            var model = new RegisterUserDto { Email = "", Password = "", ConfirmPassword = "" };
            _controller.ModelState.AddModelError("Email", "Email is required.");

            // Act
            var result = await _controller.Register(model);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Register_Post_RedirectsToHome_WhenRegistrationSucceeds()
        {
            // Arrange
            var model = new RegisterUserDto
            {
                Email = "newuser@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };
            var userDto = new UserDto { Id = "1", Email = model.Email };
            _mockUserService
                .Setup(s => s.RegisterAsync(It.IsAny<RegisterUserDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(userDto));

            // Act
            var result = await _controller.Register(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Register_Post_ReturnsViewWithError_WhenRegistrationFails()
        {
            // Arrange
            var model = new RegisterUserDto
            {
                Email = "existing@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };
            _mockUserService
                .Setup(s => s.RegisterAsync(It.IsAny<RegisterUserDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<UserDto>("A user with this email already exists."));

            // Act
            var result = await _controller.Register(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Logout_Post_RedirectsToHome()
        {
            // Arrange
            _mockUserService
                .Setup(s => s.LogoutAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());

            // Act
            var result = await _controller.Logout();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }

        [Fact]
        public void AccessDenied_ReturnsViewResult()
        {
            // Act
            var result = _controller.AccessDenied();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
