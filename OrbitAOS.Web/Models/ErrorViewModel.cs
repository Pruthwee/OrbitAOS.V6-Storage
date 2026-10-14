namespace OrbitAOS.Web.Models;

/// <summary>
/// View model for the error page.
/// Provides request tracking information for diagnostics.
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    /// <summary>
    /// Returns true when a RequestId is available to display.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
