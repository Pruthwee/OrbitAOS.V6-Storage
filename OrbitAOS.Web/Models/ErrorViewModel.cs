namespace OrbitAOS.Web.Models;

/// <summary>
/// ViewModel for the Error view.
/// Migrated from OrbitAOS.V6.Models to OrbitAOS.Web.Models namespace.
/// </summary>
public class ErrorViewModel
{
    /// <summary>Gets or sets the request ID for diagnostic purposes.</summary>
    public string? RequestId { get; set; }

    /// <summary>Gets a value indicating whether the RequestId should be displayed.</summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
