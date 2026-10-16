namespace OrbitAOS.Domain.Entities
{
    /// <summary>
    /// Represents an application user in the domain layer.
    /// Extends ASP.NET Core Identity user with domain-specific properties.
    /// </summary>
    public class ApplicationUser
    {
        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
