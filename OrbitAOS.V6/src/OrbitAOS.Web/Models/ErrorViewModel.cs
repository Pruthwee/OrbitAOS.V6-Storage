namespace OrbitAOS.Web.Models;

/// <summary>
/// View model for the error page.
/// Provides request tracking information for error diagnostics.
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Gets or sets the request ID for error tracking.
    /// Populated from Activity.Current?.Id or HttpContext.TraceIdentifier.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Gets a value indicating whether the request ID should be displayed.
    /// Returns true when RequestId is not null or empty.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
