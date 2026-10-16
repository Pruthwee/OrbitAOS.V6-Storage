// ============================================================
// Models - AppSettings
// Strongly-typed configuration class for IOptions<T> pattern.
// Replaces direct builder.Configuration access throughout the app.
// ============================================================
namespace OrbitAOS.V6.Models
{
    /// <summary>
    /// Strongly-typed application settings.
    /// Bound from the "AppSettings" section in appsettings.json.
    /// Inject via IOptions&lt;AppSettings&gt; where needed.
    /// </summary>
    public class AppSettings
    {
        /// <summary>Application display name.</summary>
        public string ApplicationName { get; set; } = "OrbitAOS";

        /// <summary>Application version string.</summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// Maximum number of items to display per page in list views.
        /// </summary>
        public int DefaultPageSize { get; set; } = 20;

        /// <summary>
        /// Feature flag: enable or disable the maintenance mode banner.
        /// </summary>
        public bool MaintenanceMode { get; set; } = false;
    }
}
