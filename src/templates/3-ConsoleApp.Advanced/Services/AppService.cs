using Microsoft.Extensions.Logging;

namespace ConsoleApp.Advanced.Services;

/// <summary>
/// Example application service demonstrating modern C# patterns.
/// </summary>
/// <remarks>
/// This service shows how to use modern C# features like primary constructors
/// (available in C# 12) and simplified logging patterns.
/// </remarks>
public class AppService(ILogger<AppService> logger)
{
//#if (UseAsync)
    /// <summary>
    /// Executes the main application logic asynchronously.
    /// </summary>
    /// <remarks>
    /// This method demonstrates modern async service execution patterns.
    /// Replace with your actual business logic following SOLID principles.
    /// </remarks>
    public async Task ExecuteAsync()
    {
        logger.LogInformation("Application service executing...");

        // Your business logic here
        await ProcessDataAsync();

        logger.LogInformation("Application service completed");
    }

    /// <summary>
    /// Example async business logic method.
    /// </summary>
    /// <remarks>
    /// Keep methods focused on single responsibilities.
    /// Extract complex logic into separate methods or services.
    /// </remarks>
    private async Task ProcessDataAsync()
    {
        logger.LogInformation("Processing data with modern patterns...");

        // Example async operation
        await Task.Delay(50);

        // Example: Modern collection processing
        var items = Enumerable.Range(1, 5);
        var processed = items.Select(x => x * 2).ToList();

        logger.LogInformation("Processed {Count} items", processed.Count);
    }
//#else
    /// <summary>
    /// Executes the main application logic.
    /// </summary>
    /// <remarks>
    /// This method demonstrates modern service execution patterns.
    /// Replace with your actual business logic following SOLID principles.
    /// </remarks>
    public void Execute()
    {
        logger.LogInformation("Application service executing...");

        // Your business logic here
        ProcessData();

        logger.LogInformation("Application service completed");
    }

    /// <summary>
    /// Example business logic method.
    /// </summary>
    /// <remarks>
    /// Keep methods focused on single responsibilities.
    /// Extract complex logic into separate methods or services.
    /// </remarks>
    private void ProcessData()
    {
        logger.LogInformation("Processing data with modern patterns...");

        // Example: Modern collection processing
        var items = Enumerable.Range(1, 5);
        var processed = items.Select(x => x * 2).ToList();

        logger.LogInformation("Processed {Count} items", processed.Count);
    }
//#endif
}
