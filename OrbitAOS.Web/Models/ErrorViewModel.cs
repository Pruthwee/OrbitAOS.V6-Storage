namespace OrbitAOS.Web.Models
{
    /// <summary>
    /// ViewModel for the Error view.
    /// Provides request tracking information for error display.
    /// </summary>
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
