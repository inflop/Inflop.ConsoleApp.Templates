//#if (UseHealthChecksBasic || UseHealthChecksAspNet)
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Advanced.Health;

/// <summary>
/// Custom health check implementation example.
/// Add your own health check logic here.
/// </summary>
public class CustomHealthCheck : IHealthCheck
{
    private readonly ILogger<CustomHealthCheck> _logger;

    public CustomHealthCheck(ILogger<CustomHealthCheck> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Add your custom health check logic here
            // Example: check if a required file exists, check memory usage, etc.
            var isHealthy = true;

            if (isHealthy)
            {
                _logger.LogDebug("Custom health check passed");
                return Task.FromResult(
                    HealthCheckResult.Healthy("Custom check passed"));
            }
            else
            {
                _logger.LogWarning("Custom health check failed");
                return Task.FromResult(
                    HealthCheckResult.Unhealthy("Custom check failed"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Custom health check threw an exception");
            return Task.FromResult(
                HealthCheckResult.Unhealthy("Exception occurred", ex));
        }
    }
}
//#endif
