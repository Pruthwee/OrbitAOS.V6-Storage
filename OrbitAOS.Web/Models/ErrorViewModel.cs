namespace OrbitAOS.Web.Models;

/// <summary>
/// View model for the error page, providing request tracking information.
/// </summary>
public class ErrorViewModel
{
    /// <summary>Gets or sets the request ID for error tracking.</summary>
    public string? RequestId { get; set; }

    /// <summary>Gets whether the request ID should be displayed.</summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
