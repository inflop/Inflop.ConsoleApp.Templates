using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Simple.Services;

/// <summary>
/// Example application service demonstrating lightweight DI pattern.
/// </summary>
/// <remarks>
/// This service shows manual dependency injection without the Generic Host overhead.
/// Useful for simple console applications where full hosting infrastructure is not needed.
/// </remarks>
public class MyService
{
    private readonly ILogger<MyService> _logger;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for diagnostic output</param>
    /// <param name="configuration">The configuration instance for application settings</param>
    public MyService(ILogger<MyService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

//#if (UseAsync)
    /// <summary>
    /// Executes the main application logic asynchronously.
    /// </summary>
    /// <remarks>
    /// This method demonstrates async service execution pattern.
    /// Replace with your actual business logic implementation.
    /// Use structured logging to provide meaningful diagnostic information.
    /// </remarks>
    public async Task RunAsync()
    {
        _logger.LogInformation("Service execution started");

        // Example: Read configuration value
        var configValue = _configuration["SampleSetting"];
        _logger.LogInformation("Configuration value: {ConfigValue}", configValue ?? "Not configured");

        // Your business logic here
        _logger.LogInformation("Processing application logic...");

        // Example async operation
        await Task.Delay(100);

        _logger.LogInformation("Service execution completed");
    }
//#else
    /// <summary>
    /// Executes the main application logic.
    /// </summary>
    /// <remarks>
    /// This method demonstrates basic service execution pattern.
    /// Replace with your actual business logic implementation.
    /// Use structured logging to provide meaningful diagnostic information.
    /// </remarks>
    public void Run()
    {
        _logger.LogInformation("Service execution started");

        // Example: Read configuration value
        var configValue = _configuration["SampleSetting"];
        _logger.LogInformation("Configuration value: {ConfigValue}", configValue ?? "Not configured");

        // Your business logic here
        _logger.LogInformation("Processing application logic...");

        _logger.LogInformation("Service execution completed");
    }
//#endif
}
