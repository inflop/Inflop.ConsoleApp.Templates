using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Advanced.Services;

/// <summary>
/// Background worker that executes application logic using modern patterns.
/// </summary>
/// <remarks>
/// This worker demonstrates modern C# features including primary constructors
/// and simplified async patterns. It executes once and then stops the application.
/// </remarks>
public class AppWorker(ILogger<AppWorker> logger, AppService appService, IHostApplicationLifetime lifetime)
    : BackgroundService
{
    /// <summary>
    /// Executes the background worker logic.
    /// </summary>
    /// <param name="stoppingToken">Cancellation token for graceful shutdown</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <remarks>
    /// This method runs the application service and handles lifecycle management.
    /// For continuous execution, replace the single run with a while loop.
    /// </remarks>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("Worker starting execution...");

//#if (UseAsync)
            await appService.ExecuteAsync();
//#else
            // NOTE: Running synchronous method in async context
            // This is acceptable for background services that don't need async operations
            appService.Execute();
//#endif

            logger.LogInformation("Worker completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during worker execution");
        }
        finally
        {
            // Stop the application after execution completes
            lifetime.StopApplication();
        }

        await Task.CompletedTask;
    }
}
