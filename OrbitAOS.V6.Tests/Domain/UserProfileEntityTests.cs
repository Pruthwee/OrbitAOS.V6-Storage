using OrbitAOS.V6.Domain.Entities;
using Xunit;

namespace OrbitAOS.V6.Tests.Domain;

/// <summary>
/// Unit tests for the <see cref="UserProfile"/> domain entity.
/// Validates entity initialization, property defaults, and business rules.
/// </summary>
public class UserProfileEntityTests
{
    /// <summary>
    /// Verifies that a new UserProfile has correct default values.
    /// </summary>
    [Fact]
    public void UserProfile_DefaultValues_AreCorrect()
    {
        // Act
        var profile = new UserProfile();

        // Assert
        Assert.True(profile.IsActive);
        Assert.Null(profile.LastLoginAt);
        Assert.Null(profile.UpdatedAt);
        Assert.Equal(string.Empty, profile.IdentityUserId);
        Assert.Equal(string.Empty, profile.DisplayName);
        Assert.Equal(string.Empty, profile.Email);
    }

    /// <summary>
    /// Verifies that UserProfile properties can be set and retrieved correctly.
    /// </summary>
    [Fact]
    public void UserProfile_Properties_CanBeSetAndRetrieved()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var profile = new UserProfile
        {
            Id = 42,
            IdentityUserId = "identity-42",
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true,
            LastLoginAt = now,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Assert
        Assert.Equal(42, profile.Id);
        Assert.Equal("identity-42", profile.IdentityUserId);
        Assert.Equal("Test User", profile.DisplayName);
        Assert.Equal("test@example.com", profile.Email);
        Assert.True(profile.IsActive);
        Assert.Equal(now, profile.LastLoginAt);
        Assert.Equal(now, profile.CreatedAt);
        Assert.Equal(now, profile.UpdatedAt);
    }

    /// <summary>
    /// Verifies that UserProfile can be deactivated.
    /// </summary>
    [Fact]
    public void UserProfile_CanBeDeactivated()
    {
        // Arrange
        var profile = new UserProfile { IsActive = true };

        // Act
        profile.IsActive = false;

        // Assert
        Assert.False(profile.IsActive);
    }
}
