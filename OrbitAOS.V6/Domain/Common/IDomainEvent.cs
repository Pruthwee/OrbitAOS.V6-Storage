// ============================================================
// Domain/Common/IDomainEvent.cs
// Clean Architecture - Domain Layer
// Domain event interface for event-driven patterns
// ============================================================

namespace OrbitAOS.V6.Domain.Common;

/// <summary>
/// Marker interface for domain events.
/// Enables event-driven communication between domain entities
/// without coupling to infrastructure concerns.
/// </summary>
public interface IDomainEvent
{
    /// <summary>UTC timestamp when the domain event occurred.</summary>
    DateTime OccurredOn { get; }
}
