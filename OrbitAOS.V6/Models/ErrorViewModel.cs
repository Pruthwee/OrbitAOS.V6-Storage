// ============================================================
// Web Layer - ErrorViewModel
// View model for the Error page.
// ============================================================
namespace OrbitAOS.V6.Models
{
    /// <summary>
    /// View model for the shared Error view.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// The current request trace identifier.
        /// Used for correlating errors in logs.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Returns <c>true</c> when <see cref="RequestId"/> has a value.
        /// Controls whether the Request ID is displayed in the Error view.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
