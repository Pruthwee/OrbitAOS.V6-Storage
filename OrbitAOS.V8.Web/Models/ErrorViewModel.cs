namespace OrbitAOS.V8.Web.Models;

/// <summary>
/// ViewModel for the Error view.
/// Migration note: Identical to legacy MVC 6.0 ErrorViewModel.
/// Nullable reference types enabled (? suffix on RequestId).
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
