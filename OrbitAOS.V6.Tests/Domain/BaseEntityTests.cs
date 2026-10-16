// ============================================================
// Tests/Domain/BaseEntityTests.cs
// Unit Tests - Domain Layer
// Tests for BaseEntity abstract class
// ============================================================

using FluentAssertions;
using OrbitAOS.V6.Domain.Entities;
using Xunit;

namespace OrbitAOS.V6.Tests.Domain;

/// <summary>
/// Concrete implementation of BaseEntity for testing purposes.
/// BaseEntity is abstract, so we need a concrete subclass.
/// </summary>
public class ConcreteEntity : BaseEntity
{
    public string? Name { get; set; }
}

/// <summary>
/// Unit tests for the BaseEntity abstract class.
/// </summary>
public class BaseEntityTests
{
    [Fact]
    public void Id_ShouldDefaultToZero()
    {
        // Act
        var entity = new ConcreteEntity();

        // Assert
        entity.Id.Should().Be(0);
    }

    [Fact]
    public void CreatedAt_ShouldDefaultToUtcNow()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var entity = new ConcreteEntity();

        // Assert
        entity.CreatedAt.Should().BeAfter(before);
        entity.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void CreatedBy_ShouldDefaultToNull()
    {
        // Act
        var entity = new ConcreteEntity();

        // Assert
        entity.CreatedBy.Should().BeNull();
    }

    [Fact]
    public void UpdatedAt_ShouldDefaultToNull()
    {
        // Act
        var entity = new ConcreteEntity();

        // Assert
        entity.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void UpdatedBy_ShouldDefaultToNull()
    {
        // Act
        var entity = new ConcreteEntity();

        // Assert
        entity.UpdatedBy.Should().BeNull();
    }

    [Fact]
    public void IsDeleted_ShouldDefaultToFalse()
    {
        // Act
        var entity = new ConcreteEntity();

        // Assert
        entity.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void BaseEntity_ShouldAllowSettingAllAuditFields()
    {
        // Arrange
        var now = DateTime.UtcNow;

        // Act
        var entity = new ConcreteEntity
        {
            Id = 1,
            CreatedAt = now,
            CreatedBy = "user-1",
            UpdatedAt = now,
            UpdatedBy = "user-2",
            IsDeleted = true
        };

        // Assert
        entity.Id.Should().Be(1);
        entity.CreatedAt.Should().Be(now);
        entity.CreatedBy.Should().Be("user-1");
        entity.UpdatedAt.Should().Be(now);
        entity.UpdatedBy.Should().Be("user-2");
        entity.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void BaseEntity_ShouldSupportSoftDelete()
    {
        // Arrange
        var entity = new ConcreteEntity { Id = 5, Name = "Test" };

        // Act
        entity.IsDeleted = true;

        // Assert
        entity.IsDeleted.Should().BeTrue();
        entity.Id.Should().Be(5);
    }

    [Fact]
    public void ConcreteEntity_ShouldInheritBaseEntityProperties()
    {
        // Act
        var entity = new ConcreteEntity { Name = "Test Entity" };

        // Assert
        entity.Should().BeAssignableTo<BaseEntity>();
        entity.Name.Should().Be("Test Entity");
    }
}
