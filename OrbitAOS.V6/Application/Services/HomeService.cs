// ============================================================
// Application Layer - Home Service Implementation
// Contains business logic for the Home feature.
// ============================================================
using OrbitAOS.V6.Application.Interfaces;

namespace OrbitAOS.V6.Application.Services
{
    /// <summary>
    /// Implements home-page business logic.
    /// </summary>
    public class HomeService : IHomeService
    {
        private readonly ILogger<HomeService> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="HomeService"/>.
        /// </summary>
        /// <param name="logger">Logger injected via DI.</param>
        public HomeService(ILogger<HomeService> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public string GetWelcomeMessage()
        {
            _logger.LogInformation("HomeService.GetWelcomeMessage called.");
            return "Welcome to OrbitAOS on .NET 8!";
        }
    }
}
