// ============================================================
// Infrastructure/Data/Migrations/README.md (as .cs placeholder)
// Infrastructure/Data/Migrations/MigrationNotes.cs
// Clean Architecture - Infrastructure Layer
// Migration notes for EF Core 10 upgrade
// ============================================================

namespace OrbitAOS.V6.Infrastructure.Data.Migrations;

/// <summary>
/// EF Core Migration Notes for OrbitAOS.V6 (.NET 10 upgrade).
///
/// IMPORTANT: The existing migration file (00000000000000_CreateIdentitySchema.cs)
/// was generated targeting EF Core 3.0 (ProductVersion: "3.0.0") and uses
/// deprecated API patterns (nullable: false, HasName, HasMaxLength).
///
/// MIGRATION STRATEGY for .NET 10:
/// -------------------------------------------------------
/// Option A - Regenerate migrations (RECOMMENDED for clean upgrade):
///   1. Delete all files in Data/Migrations/ folder
///   2. Run: dotnet ef migrations add InitialCreate --project OrbitAOS.V6
///   3. Run: dotnet ef database update
///
/// Option B - Keep existing migrations (if database already deployed):
///   1. The existing migration schema is compatible with EF Core 10
///   2. Update the Designer.cs files to remove deprecated annotations
///   3. Run: dotnet ef database update to apply any pending migrations
///
/// BREAKING CHANGES in EF Core 10 vs EF Core 3.0:
/// - HasName() → HasDatabaseName() for index naming
/// - HasMaxLength() on Property → MaxLength() attribute or HasMaxLength() in fluent API
/// - nullable: false → IsRequired() in fluent API
/// - ProductVersion annotation must be updated to current EF Core version
/// - SqlServerValueGenerationStrategy.IdentityColumn → UseIdentityColumn()
///
/// SCAFFOLD COMMANDS for Database-First approach:
/// -------------------------------------------------------
/// dotnet ef dbcontext scaffold \
///   "Server=.;Database=OrbitAOS;Trusted_Connection=True;" \
///   Microsoft.EntityFrameworkCore.SqlServer \
///   --output-dir Infrastructure/Data/Entities \
///   --context-dir Infrastructure/Data \
///   --context ApplicationDbContext \
///   --namespace OrbitAOS.V6.Infrastructure.Data \
///   --data-annotations \
///   --force
///
/// FUTURE MIGRATION COMMANDS:
/// -------------------------------------------------------
/// Add migration:    dotnet ef migrations add <MigrationName>
/// Apply migration:  dotnet ef database update
/// Revert migration: dotnet ef database update <PreviousMigrationName>
/// List migrations:  dotnet ef migrations list
/// Generate SQL:     dotnet ef migrations script
/// </summary>
public static class MigrationNotes
{
    public const string EfCoreVersion = "10.0.0";
    public const string TargetDatabase = "SQL Server";
    public const string MigrationStrategy = "Code-First with Identity Schema";
}
