using ConsoleApp.Enterprise.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConsoleApp.Enterprise.Services;

/// <summary>
/// Secondary background worker demonstrating continuous execution pattern.
/// </summary>
/// <remarks>
/// This worker demonstrates how to implement long-running background tasks
/// with configurable execution intervals. Useful for monitoring, polling,
/// or periodic maintenance tasks.
/// </remarks>
public class SecondaryWorker : BackgroundService
{
    private readonly ILogger<SecondaryWorker> _logger;
    private readonly WorkerSettings _workerSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecondaryWorker"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for diagnostic output</param>
    /// <param name="workerSettings">Strongly-typed worker settings</param>
    public SecondaryWorker(ILogger<SecondaryWorker> logger, IOptions<WorkerSettings> workerSettings)
    {
        _logger = logger;
        _workerSettings = workerSettings.Value;
    }

    /// <summary>
    /// Executes the secondary worker logic continuously.
    /// </summary>
    /// <param name="stoppingToken">Cancellation token for graceful shutdown</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <remarks>
    /// This method demonstrates continuous execution with configurable intervals.
    /// The worker respects cancellation tokens for graceful shutdown.
    /// Disable by setting ContinuousExecution to false in configuration.
    /// </remarks>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_workerSettings.ContinuousExecution)
        {
            _logger.LogInformation("Secondary worker disabled via configuration");
            return;
        }

        _logger.LogInformation("Secondary worker starting with {IntervalMs}ms interval", _workerSettings.ExecutionIntervalMs);

        var iteration = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                iteration++;
                _logger.LogDebug("Secondary worker iteration {Iteration}", iteration);

                // Your background task logic here
                await Task.Delay(TimeSpan.FromMilliseconds(_workerSettings.ExecutionIntervalMs), stoppingToken);

                // Check max iterations limit
                if (_workerSettings.MaxIterations > 0 && iteration >= _workerSettings.MaxIterations)
                {
                    _logger.LogInformation("Secondary worker reached max iterations: {MaxIterations}",
                        _workerSettings.MaxIterations);
                    break;
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, "Secondary worker cancellation requested");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Secondary worker error on iteration {Iteration}", iteration);
                // Continue execution despite errors
            }
        }

        _logger.LogInformation("Secondary worker stopped after {Iterations} iterations", iteration);
    }
}
