// ============================================================
// Models/ErrorViewModel.cs
// Backward-compatibility shim for OrbitAOS.V6.Models namespace
// Clean Architecture: ViewModels live in Web/ViewModels/
// This file provides the Models namespace for any legacy references
// ============================================================

namespace OrbitAOS.V6.Models;

/// <summary>
/// Backward-compatibility alias for ErrorViewModel.
/// The canonical implementation is in Web/ViewModels/ErrorViewModel.cs.
/// This class is kept to satisfy any legacy @using OrbitAOS.V6.Models references.
/// </summary>
public class ErrorViewModel : OrbitAOS.V6.Web.ViewModels.ErrorViewModel
{
}
