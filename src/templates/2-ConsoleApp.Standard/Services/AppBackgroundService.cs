using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Standard.Services;

/// <summary>
/// Background service that executes application logic using the Generic Host pattern.
/// </summary>
/// <remarks>
/// BackgroundService provides lifecycle management and graceful shutdown handling.
/// This service executes once on startup and then stops the application.
/// For long-running services, modify ExecuteAsync to run continuously.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="AppBackgroundService"/> class.
/// </remarks>
/// <param name="logger">The logger instance for diagnostic output</param>
/// <param name="appService">The application service to execute</param>
/// <param name="lifetime">The application lifetime for graceful shutdown</param>
public class AppBackgroundService(ILogger<AppBackgroundService> logger, AppService appService, IHostApplicationLifetime lifetime)
    : BackgroundService
{
    private readonly ILogger<AppBackgroundService> _logger = logger;
    private readonly AppService _appService = appService;
    private readonly IHostApplicationLifetime _lifetime = lifetime;

    /// <summary>
    /// Executes the background service logic.
    /// </summary>
    /// <param name="stoppingToken">Cancellation token for graceful shutdown</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <remarks>
    /// This method runs the application service and then stops the host.
    /// For continuous execution, replace the single run with a while loop
    /// that checks stoppingToken.IsCancellationRequested.
    /// </remarks>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Background service starting...");

//#if (UseAsync)
            await _appService.RunAsync();
//#else
            // NOTE: Running synchronous method in async context
            // This is acceptable for background services that don't need async operations
            _appService.Run();
//#endif

            _logger.LogInformation("Background service completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during execution");
        }
        finally
        {
            // Stop the application after execution completes
            _lifetime.StopApplication();
        }

        await Task.CompletedTask;
    }
}
