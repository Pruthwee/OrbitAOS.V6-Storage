// ============================================================
// Application/Common/Interfaces/IDateTimeService.cs
// Clean Architecture - Application Layer
// Abstraction over DateTime for testability
// ============================================================

namespace OrbitAOS.V6.Application.Common.Interfaces;

/// <summary>
/// Service interface for date/time operations.
/// Abstracts DateTime.Now/UtcNow to enable deterministic unit testing.
/// </summary>
public interface IDateTimeService
{
    /// <summary>Gets the current UTC date and time.</summary>
    DateTime UtcNow { get; }

    /// <summary>Gets the current local date and time.</summary>
    DateTime Now { get; }

    /// <summary>Gets today's UTC date (time component is midnight).</summary>
    DateOnly Today { get; }
}
