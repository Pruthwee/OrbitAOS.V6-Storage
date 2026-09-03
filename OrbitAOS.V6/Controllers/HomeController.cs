using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using OrbitAOS.V6.Models;
using System.Diagnostics;
using System.Text.Json;

namespace OrbitAOS.V6.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDistributedCache _cache;

        public HomeController(ILogger<HomeController> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        // cr-dotnet-1000: Converted to async Task<IActionResult> with AWS ElastiCache (Redis)
        // non-blocking data retrieval pattern for cloud scalability.
        public async Task<IActionResult> Index()
        {
            const string cacheKey = "home:index:data";
            string? cachedData = await _cache.GetStringAsync(cacheKey);

            if (cachedData == null)
            {
                _logger.LogInformation("Cache miss for key '{CacheKey}'. Fetching data.", cacheKey);
                // Simulate async data retrieval (replace with actual async data source as needed)
                var data = new { Message = "Welcome to OrbitAOS", Timestamp = DateTime.UtcNow };
                cachedData = JsonSerializer.Serialize(data);

                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };
                await _cache.SetStringAsync(cacheKey, cachedData, cacheOptions);
            }
            else
            {
                _logger.LogInformation("Cache hit for key '{CacheKey}'.", cacheKey);
            }

            return View();
        }

        // cr-dotnet-1000: Converted to async Task<IActionResult> with AWS ElastiCache (Redis)
        // non-blocking data retrieval pattern for cloud scalability.
        public async Task<IActionResult> Privacy()
        {
            const string cacheKey = "home:privacy:data";
            string? cachedData = await _cache.GetStringAsync(cacheKey);

            if (cachedData == null)
            {
                _logger.LogInformation("Cache miss for key '{CacheKey}'. Fetching data.", cacheKey);
                var data = new { Page = "Privacy", Timestamp = DateTime.UtcNow };
                cachedData = JsonSerializer.Serialize(data);

                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };
                await _cache.SetStringAsync(cacheKey, cachedData, cacheOptions);
            }
            else
            {
                _logger.LogInformation("Cache hit for key '{CacheKey}'.", cacheKey);
            }

            return View();
        }

        // cr-dotnet-1000: Converted to async Task<IActionResult> with AWS ElastiCache (Redis)
        // non-blocking data retrieval pattern for cloud scalability.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            const string cacheKey = "home:error:config";
            string? cachedConfig = await _cache.GetStringAsync(cacheKey);

            if (cachedConfig == null)
            {
                _logger.LogInformation("Cache miss for key '{CacheKey}'.", cacheKey);
                var config = new { ShowDetails = false };
                cachedConfig = JsonSerializer.Serialize(config);

                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                };
                await _cache.SetStringAsync(cacheKey, cachedConfig, cacheOptions);
            }

            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}
