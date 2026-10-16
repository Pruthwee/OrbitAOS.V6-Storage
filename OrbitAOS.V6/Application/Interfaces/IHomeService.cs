// ============================================================
// Application Layer - Home Service Interface
// Defines the contract for home-page business logic.
// ============================================================
namespace OrbitAOS.V6.Application.Interfaces
{
    /// <summary>
    /// Service interface for the Home feature.
    /// </summary>
    public interface IHomeService
    {
        /// <summary>Returns the welcome message for the home page.</summary>
        string GetWelcomeMessage();
    }
}
