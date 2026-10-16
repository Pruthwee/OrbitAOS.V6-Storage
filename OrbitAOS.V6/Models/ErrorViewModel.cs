namespace OrbitAOS.V6.Models;

/// <summary>
/// ViewModel for the Error view.
/// Upgraded from net6.0 to net8.0 - file-scoped namespace applied (C# 10+).
/// </summary>
public class ErrorViewModel
{
    /// <summary>Gets or sets the request ID for diagnostic purposes.</summary>
    public string? RequestId { get; set; }

    /// <summary>Gets a value indicating whether the RequestId should be displayed.</summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
