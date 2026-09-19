namespace OrbitAOS.V6.Web.Models;

/// <summary>
/// View model for the Error page.
/// Carries the request ID to help with diagnostics and support.
/// </summary>
public class ErrorViewModel
{
    /// <summary>Gets or sets the current request ID for diagnostic purposes.</summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Gets a value indicating whether the request ID should be displayed.
    /// Returns true when <see cref="RequestId"/> is not null or empty.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
