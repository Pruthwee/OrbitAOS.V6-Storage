namespace OrbitAOS.V6.Models;

/// <summary>
/// View model for the error page.
/// Updated to use file-scoped namespace (C# 10+ feature available in .NET 8).
/// </summary>
public class ErrorViewModel
{
    /// <summary>Gets or sets the request ID for error tracking.</summary>
    public string? RequestId { get; set; }

    /// <summary>Gets a value indicating whether the request ID should be displayed.</summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
