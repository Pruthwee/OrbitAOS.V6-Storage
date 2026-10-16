namespace OrbitAOS.Domain.Entities
{
    /// <summary>
    /// Base entity class providing common identity properties for all domain entities.
    /// </summary>
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
