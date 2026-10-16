// ============================================================
// Tests/Web/ViewModels/UserViewModelTests.cs
// Unit Tests - Web Layer ViewModels
// Tests for UserViewModel
// ============================================================

using FluentAssertions;
using OrbitAOS.V6.Web.ViewModels;
using Xunit;

namespace OrbitAOS.V6.Tests.Web.ViewModels;

/// <summary>
/// Unit tests for UserViewModel.
/// </summary>
public class UserViewModelTests
{
    [Fact]
    public void UserViewModel_DefaultConstructor_ShouldInitializeRoles()
    {
        // Act
        var model = new UserViewModel();

        // Assert
        model.Roles.Should().NotBeNull();
        model.Roles.Should().BeEmpty();
    }

    [Fact]
    public void Id_ShouldDefaultToNull()
    {
        // Act
        var model = new UserViewModel();

        // Assert
        model.Id.Should().BeNull();
    }

    [Fact]
    public void FullName_ShouldDefaultToNull()
    {
        // Act
        var model = new UserViewModel();

        // Assert
        model.FullName.Should().BeNull();
    }

    [Fact]
    public void Email_ShouldDefaultToNull()
    {
        // Act
        var model = new UserViewModel();

        // Assert
        model.Email.Should().BeNull();
    }

    [Fact]
    public void IsActive_ShouldDefaultToFalse()
    {
        // Act
        var model = new UserViewModel();

        // Assert
        model.IsActive.Should().BeFalse();
    }

    [Fact]
    public void LastLoginAt_ShouldDefaultToNull()
    {
        // Act
        var model = new UserViewModel();

        // Assert
        model.LastLoginAt.Should().BeNull();
    }

    [Fact]
    public void UserViewModel_ShouldAllowSettingAllProperties()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var roles = new List<string> { "Admin", "User" };

        // Act
        var model = new UserViewModel
        {
            Id = "user-id-123",
            FullName = "John Doe",
            Email = "john@example.com",
            IsActive = true,
            Roles = roles,
            LastLoginAt = now
        };

        // Assert
        model.Id.Should().Be("user-id-123");
        model.FullName.Should().Be("John Doe");
        model.Email.Should().Be("john@example.com");
        model.IsActive.Should().BeTrue();
        model.Roles.Should().BeEquivalentTo(roles);
        model.LastLoginAt.Should().Be(now);
    }

    [Fact]
    public void Roles_ShouldAllowAddingItems()
    {
        // Arrange
        var model = new UserViewModel();

        // Act
        model.Roles.Add("Administrator");
        model.Roles.Add("Manager");

        // Assert
        model.Roles.Should().HaveCount(2);
        model.Roles.Should().Contain("Administrator");
        model.Roles.Should().Contain("Manager");
    }

    [Fact]
    public void UserViewModel_WithActiveUser_ShouldHaveCorrectState()
    {
        // Act
        var model = new UserViewModel
        {
            Id = "abc",
            FullName = "Active User",
            Email = "active@example.com",
            IsActive = true
        };

        // Assert
        model.IsActive.Should().BeTrue();
        model.Email.Should().NotBeNullOrEmpty();
    }
}
