// ============================================================
// Domain Layer - Base Entity
// All domain entities inherit from this base class.
// ============================================================
namespace OrbitAOS.V6.Domain.Entities
{
    /// <summary>
    /// Base entity with a generic primary key.
    /// </summary>
    /// <typeparam name="TKey">Type of the primary key.</typeparam>
    public abstract class BaseEntity<TKey>
    {
        /// <summary>Primary key identifier.</summary>
        public TKey Id { get; set; } = default!;

        /// <summary>UTC timestamp when the record was created.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>UTC timestamp when the record was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Convenience base entity with an integer primary key.
    /// </summary>
    public abstract class BaseEntity : BaseEntity<int> { }
}
