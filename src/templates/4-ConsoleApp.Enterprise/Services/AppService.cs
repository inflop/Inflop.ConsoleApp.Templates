using ConsoleApp.Enterprise.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConsoleApp.Enterprise.Services;

/// <summary>
/// Main application service implementing business logic.
/// </summary>
/// <remarks>
/// This service demonstrates enterprise patterns including strongly-typed configuration,
/// dependency injection, structured logging, and async operations.
/// Follow SOLID principles: keep the service focused on a single responsibility.
/// </remarks>
public class AppService : IAppService
{
    private readonly ILogger<AppService> _logger;
    private readonly AppSettings _appSettings;
    private readonly IDataProcessor _dataProcessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for diagnostic output</param>
    /// <param name="appSettings">Strongly-typed application settings</param>
    /// <param name="dataProcessor">Data processor service for business logic</param>
    public AppService(
        ILogger<AppService> logger,
        IOptions<AppSettings> appSettings,
        IDataProcessor dataProcessor)
    {
        _logger = logger;
        _appSettings = appSettings.Value;
        _dataProcessor = dataProcessor;
    }

//#if (UseAsync)
    /// <summary>
    /// Executes the main application logic asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for graceful shutdown</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <remarks>
    /// This method coordinates business logic execution with proper error handling
    /// and cancellation support. Use structured logging for observability.
    /// </remarks>
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Application service starting: {AppName} v{Version}",
            _appSettings.ApplicationName,
            _appSettings.Version);

        try
        {
            // Execute business logic with cancellation support
            await _dataProcessor.ProcessAsync(cancellationToken);

            _logger.LogInformation("Application service completed successfully");
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Application service cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during application execution");
            throw;
        }
    }
//#else
    /// <summary>
    /// Executes the main application logic.
    /// </summary>
    /// <remarks>
    /// This method coordinates business logic execution with proper error handling.
    /// Use structured logging for observability.
    /// </remarks>
    public void Execute()
    {
        _logger.LogInformation("Application service starting: {AppName} v{Version}",
            _appSettings.ApplicationName,
            _appSettings.Version);

        try
        {
            // Execute business logic
            _dataProcessor.Process();

            _logger.LogInformation("Application service completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during application execution");
            throw;
        }
    }
//#endif
}
