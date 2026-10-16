// ============================================================
// Application/Common/Interfaces/IApplicationDbContext.cs
// Clean Architecture - Application Layer
// DbContext interface for dependency inversion
// ============================================================

using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Domain.Entities;

namespace OrbitAOS.V6.Application.Common.Interfaces;

/// <summary>
/// Application-layer abstraction over the EF Core DbContext.
/// Enables testability by allowing mock implementations in unit tests.
/// The Application layer depends on this interface, not the concrete DbContext.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>Saves all pending changes to the database.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
