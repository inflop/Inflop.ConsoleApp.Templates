using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Enterprise.Services;

/// <summary>
/// Primary background worker for application execution.
/// </summary>
/// <remarks>
/// This worker demonstrates the primary execution pattern for console applications.
/// It executes the main business logic and then stops the application.
/// For continuous execution, see SecondaryWorker.
/// </remarks>
public class PrimaryWorker : BackgroundService
{
    private readonly ILogger<PrimaryWorker> _logger;
    private readonly IAppService _appService;
    private readonly IHostApplicationLifetime _lifetime;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrimaryWorker"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for diagnostic output</param>
    /// <param name="appService">The application service to execute</param>
    /// <param name="lifetime">The application lifetime for shutdown control</param>
    public PrimaryWorker(ILogger<PrimaryWorker> logger, IAppService appService, IHostApplicationLifetime lifetime)
    {
        _logger = logger;
        _appService = appService;
        _lifetime = lifetime;
    }

    /// <summary>
    /// Executes the primary worker logic.
    /// </summary>
    /// <param name="stoppingToken">Cancellation token for graceful shutdown</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <remarks>
    /// This method executes the application service and handles lifecycle management.
    /// Proper exception handling and logging ensure production reliability.
    /// </remarks>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Primary worker starting...");

//#if (UseAsync)
            await _appService.ExecuteAsync(stoppingToken);
//#else
            // NOTE: Running synchronous method in async context
            // This is acceptable for background services that don't need async operations
            _appService.Execute();
//#endif

            _logger.LogInformation("Primary worker completed");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Primary worker was canceled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Primary worker encountered an error");
        }
        finally
        {
            // Stop the application after execution completes
            _lifetime.StopApplication();
        }
    }
}
