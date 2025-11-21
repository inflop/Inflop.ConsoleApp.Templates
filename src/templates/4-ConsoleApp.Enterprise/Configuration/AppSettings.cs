namespace ConsoleApp.Enterprise.Configuration;

/// <summary>
/// Strongly-typed application settings from configuration.
/// </summary>
/// <remarks>
/// This class demonstrates the Options pattern for type-safe configuration.
/// Bind this to a configuration section using Configure&lt;AppSettings&gt;.
/// This approach provides IntelliSense, compile-time checking, and clear documentation.
/// </remarks>
public class AppSettings
{
    /// <summary>
    /// Gets or sets the application name.
    /// </summary>
    public string ApplicationName { get; set; } = "Enterprise Console App";

    /// <summary>
    /// Gets or sets the application version.
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Gets or sets the maximum retry attempts for operations.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Gets or sets the timeout in seconds for operations.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets a value indicating whether debug mode is enabled.
    /// </summary>
    public bool EnableDebugMode { get; set; } = false;
}
