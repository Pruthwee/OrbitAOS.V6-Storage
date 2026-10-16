// ============================================================
// Infrastructure/Services/DateTimeService.cs
// Clean Architecture - Infrastructure Layer
// Implements IDateTimeService for testable date/time access
// ============================================================

using OrbitAOS.V6.Application.Common.Interfaces;

namespace OrbitAOS.V6.Infrastructure.Services;

/// <summary>
/// Concrete implementation of IDateTimeService.
/// Wraps DateTime.UtcNow to allow mocking in unit tests.
/// </summary>
public class DateTimeService : IDateTimeService
{
    /// <inheritdoc/>
    public DateTime UtcNow => DateTime.UtcNow;

    /// <inheritdoc/>
    public DateTime Now => DateTime.Now;

    /// <inheritdoc/>
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}
