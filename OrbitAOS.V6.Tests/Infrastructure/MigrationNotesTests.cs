// ============================================================
// Tests/Infrastructure/MigrationNotesTests.cs
// Unit Tests - Infrastructure Layer
// Tests for MigrationNotes static class
// ============================================================

using FluentAssertions;
using OrbitAOS.V6.Infrastructure.Data.Migrations;
using Xunit;

namespace OrbitAOS.V6.Tests.Infrastructure;

/// <summary>
/// Unit tests for MigrationNotes static class.
/// Verifies that migration constants are correctly defined.
/// </summary>
public class MigrationNotesTests
{
    [Fact]
    public void EfCoreVersion_ShouldBe10()
    {
        // Assert
        MigrationNotes.EfCoreVersion.Should().Be("10.0.0");
    }

    [Fact]
    public void TargetDatabase_ShouldBeSqlServer()
    {
        // Assert
        MigrationNotes.TargetDatabase.Should().Be("SQL Server");
    }

    [Fact]
    public void MigrationStrategy_ShouldBeCodeFirst()
    {
        // Assert
        MigrationNotes.MigrationStrategy.Should().Contain("Code-First");
    }

    [Fact]
    public void EfCoreVersion_ShouldNotBeNullOrEmpty()
    {
        // Assert
        MigrationNotes.EfCoreVersion.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void TargetDatabase_ShouldNotBeNullOrEmpty()
    {
        // Assert
        MigrationNotes.TargetDatabase.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void MigrationStrategy_ShouldNotBeNullOrEmpty()
    {
        // Assert
        MigrationNotes.MigrationStrategy.Should().NotBeNullOrEmpty();
    }
}
