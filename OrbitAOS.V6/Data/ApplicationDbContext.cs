// ============================================================
// Data/ApplicationDbContext.cs
// MIGRATION NOTE: This file has been superseded by the Clean Architecture
// Infrastructure layer implementation at:
//   Infrastructure/Data/ApplicationDbContext.cs
//
// This file is kept as a namespace alias for backward compatibility
// during the migration transition period.
//
// In .NET 10 Clean Architecture:
// - DbContext lives in Infrastructure/Data/ApplicationDbContext.cs
// - It implements IApplicationDbContext (Application layer interface)
// - It uses ApplicationUser (Domain entity) instead of IdentityUser
// ============================================================

// Re-export the Infrastructure DbContext under the legacy namespace
// so any remaining references to OrbitAOS.V6.Data.ApplicationDbContext still compile
global using OrbitAOS.V6.Infrastructure.Data;
