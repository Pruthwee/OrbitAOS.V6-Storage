// ============================================================
// Web/ViewModels/ErrorViewModel.cs
// Clean Architecture - Web Layer
// Moved from Models/ to Web/ViewModels/ per Clean Architecture
// ============================================================

namespace OrbitAOS.V6.Web.ViewModels;

/// <summary>
/// ViewModel for the Error view.
/// Moved from Models/ErrorViewModel.cs to Web/ViewModels/ per Clean Architecture.
/// ViewModels belong in the Web layer, not the Domain/Application layer.
/// </summary>
public class ErrorViewModel
{
    /// <summary>The ASP.NET Core request trace identifier.</summary>
    public string? RequestId { get; set; }

    /// <summary>Indicates whether the RequestId should be displayed.</summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
