using Microsoft.Extensions.Logging;

namespace ConsoleApp.Standard.Services;

/// <summary>
/// Example application service demonstrating dependency injection and logging.
/// </summary>
/// <remarks>
/// This service shows how to use ILogger for structured logging and how to
/// implement business logic in a testable, maintainable way following SOLID principles.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="AppService"/> class.
/// </remarks>
/// <param name="logger">The logger instance for structured logging</param>
public class AppService(ILogger<AppService> logger)
{
    private readonly ILogger<AppService> _logger = logger;

//#if (UseAsync)
    /// <summary>
    /// Executes the main application logic asynchronously.
    /// </summary>
    /// <remarks>
    /// This method demonstrates async logging patterns and can be replaced
    /// with your actual business logic. Use structured logging with log levels
    /// to provide meaningful diagnostic information.
    /// </remarks>
    public async Task RunAsync()
    {
        _logger.LogInformation("Application service started");

        // Your business logic here
        _logger.LogInformation("Processing application logic...");

        // Example async operation
        await Task.Delay(100);

        _logger.LogInformation("Application service completed successfully");
    }
//#else
    /// <summary>
    /// Executes the main application logic.
    /// </summary>
    /// <remarks>
    /// This method demonstrates basic logging patterns and can be replaced
    /// with your actual business logic. Use structured logging with log levels
    /// to provide meaningful diagnostic information.
    /// </remarks>
    public void Run()
    {
        _logger.LogInformation("Application service started");

        // Your business logic here
        _logger.LogInformation("Processing application logic...");

        _logger.LogInformation("Application service completed successfully");
    }
//#endif
}
