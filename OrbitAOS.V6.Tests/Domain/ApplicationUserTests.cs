// ============================================================
// Tests/Domain/ApplicationUserTests.cs
// Unit Tests - Domain Layer
// Tests for ApplicationUser entity
// ============================================================

using FluentAssertions;
using OrbitAOS.V6.Domain.Entities;
using Xunit;

namespace OrbitAOS.V6.Tests.Domain;

/// <summary>
/// Unit tests for the ApplicationUser domain entity.
/// </summary>
public class ApplicationUserTests
{
    [Fact]
    public void DisplayName_ShouldCombineFirstAndLastName()
    {
        // Arrange
        var user = new ApplicationUser
        {
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var displayName = user.DisplayName;

        // Assert
        displayName.Should().Be("John Doe");
    }

    [Fact]
    public void DisplayName_ShouldReturnFirstName_WhenLastNameIsNull()
    {
        // Arrange
        var user = new ApplicationUser
        {
            FirstName = "John",
            LastName = null
        };

        // Act
        var displayName = user.DisplayName;

        // Assert
        displayName.Should().Be("John");
    }

    [Fact]
    public void DisplayName_ShouldReturnLastName_WhenFirstNameIsNull()
    {
        // Arrange
        var user = new ApplicationUser
        {
            FirstName = null,
            LastName = "Doe"
        };

        // Act
        var displayName = user.DisplayName;

        // Assert
        displayName.Should().Be("Doe");
    }

    [Fact]
    public void DisplayName_ShouldReturnEmpty_WhenBothNamesAreNull()
    {
        // Arrange
        var user = new ApplicationUser
        {
            FirstName = null,
            LastName = null
        };

        // Act
        var displayName = user.DisplayName;

        // Assert
        displayName.Should().BeEmpty();
    }

    [Fact]
    public void DisplayName_ShouldTrimWhitespace_WhenOneNameIsEmpty()
    {
        // Arrange
        var user = new ApplicationUser
        {
            FirstName = "Alice",
            LastName = ""
        };

        // Act
        var displayName = user.DisplayName;

        // Assert
        displayName.Should().Be("Alice");
    }

    [Fact]
    public void IsActive_ShouldDefaultToTrue()
    {
        // Act
        var user = new ApplicationUser();

        // Assert
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void CreatedAt_ShouldDefaultToUtcNow()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var user = new ApplicationUser();

        // Assert
        user.CreatedAt.Should().BeAfter(before);
        user.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void LastLoginAt_ShouldDefaultToNull()
    {
        // Act
        var user = new ApplicationUser();

        // Assert
        user.LastLoginAt.Should().BeNull();
    }

    [Fact]
    public void ProfilePictureUrl_ShouldDefaultToNull()
    {
        // Act
        var user = new ApplicationUser();

        // Assert
        user.ProfilePictureUrl.Should().BeNull();
    }

    [Fact]
    public void ApplicationUser_ShouldAllowSettingAllProperties()
    {
        // Arrange
        var now = DateTime.UtcNow;

        // Act
        var user = new ApplicationUser
        {
            FirstName = "Jane",
            LastName = "Smith",
            IsActive = false,
            CreatedAt = now,
            LastLoginAt = now,
            ProfilePictureUrl = "https://example.com/pic.jpg"
        };

        // Assert
        user.FirstName.Should().Be("Jane");
        user.LastName.Should().Be("Smith");
        user.IsActive.Should().BeFalse();
        user.CreatedAt.Should().Be(now);
        user.LastLoginAt.Should().Be(now);
        user.ProfilePictureUrl.Should().Be("https://example.com/pic.jpg");
    }
}
