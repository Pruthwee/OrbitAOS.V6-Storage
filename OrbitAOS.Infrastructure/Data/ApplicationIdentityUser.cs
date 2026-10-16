using Microsoft.AspNetCore.Identity;

namespace OrbitAOS.Infrastructure.Data
{
    /// <summary>
    /// Extended Identity user class for the application.
    /// Adds domain-specific properties to the standard ASP.NET Core Identity user.
    /// Replaces the legacy ASP.NET Identity ApplicationUser from MVC 6.0.
    /// </summary>
    public class ApplicationIdentityUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
