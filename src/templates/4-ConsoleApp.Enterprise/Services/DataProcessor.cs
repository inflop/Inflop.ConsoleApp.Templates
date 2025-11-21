using ConsoleApp.Enterprise.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConsoleApp.Enterprise.Services;

/// <summary>
/// Service responsible for data processing operations.
/// </summary>
/// <remarks>
/// This service demonstrates separation of concerns by isolating data processing
/// logic from coordination logic. Each service has a single, well-defined responsibility.
/// </remarks>
public class DataProcessor : IDataProcessor
{
    private readonly ILogger<DataProcessor> _logger;
    private readonly AppSettings _appSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataProcessor"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for diagnostic output</param>
    /// <param name="appSettings">Strongly-typed application settings</param>
    public DataProcessor(
        ILogger<DataProcessor> logger,
        IOptions<AppSettings> appSettings)
    {
        _logger = logger;
        _appSettings = appSettings.Value;
    }

//#if (UseAsync)
    /// <summary>
    /// Processes data with retry logic and cancellation support.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for operation cancellation</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <remarks>
    /// This method demonstrates async processing patterns including retry logic,
    /// cancellation support, and structured logging for production scenarios.
    /// </remarks>
    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting data processing with max {MaxRetries} retries",
            _appSettings.MaxRetryAttempts);

        var attempt = 0;
        while (attempt < _appSettings.MaxRetryAttempts)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                attempt++;
                _logger.LogDebug("Processing attempt {Attempt} of {MaxAttempts}",
                    attempt, _appSettings.MaxRetryAttempts);

                // Simulate async work
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

                // Your actual data processing logic here
                _logger.LogInformation("Data processed successfully");
                return;
            }
            catch (Exception ex) when (attempt < _appSettings.MaxRetryAttempts)
            {
                _logger.LogWarning(ex, "Processing failed on attempt {Attempt}, retrying...", attempt);
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }

        _logger.LogError("Data processing failed after {MaxAttempts} attempts",
            _appSettings.MaxRetryAttempts);
        throw new InvalidOperationException($"Processing failed after {_appSettings.MaxRetryAttempts} attempts");
    }
//#else
    /// <summary>
    /// Processes data with retry logic.
    /// </summary>
    /// <remarks>
    /// This method demonstrates synchronous processing patterns including retry logic
    /// and structured logging for production scenarios.
    /// </remarks>
    public void Process()
    {
        _logger.LogInformation("Starting data processing with max {MaxRetries} retries",
            _appSettings.MaxRetryAttempts);

        var attempt = 0;
        while (attempt < _appSettings.MaxRetryAttempts)
        {
            try
            {
                attempt++;
                _logger.LogDebug("Processing attempt {Attempt} of {MaxAttempts}",
                    attempt, _appSettings.MaxRetryAttempts);

                // Simulate work
                Thread.Sleep(TimeSpan.FromSeconds(1));

                // Your actual data processing logic here
                _logger.LogInformation("Data processed successfully");
                return;
            }
            catch (Exception ex) when (attempt < _appSettings.MaxRetryAttempts)
            {
                _logger.LogWarning(ex, "Processing failed on attempt {Attempt}, retrying...", attempt);
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
        }

        _logger.LogError("Data processing failed after {MaxAttempts} attempts",
            _appSettings.MaxRetryAttempts);
        throw new InvalidOperationException($"Processing failed after {_appSettings.MaxRetryAttempts} attempts");
    }
//#endif
}
