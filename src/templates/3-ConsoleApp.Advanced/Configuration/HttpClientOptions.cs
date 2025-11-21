namespace ConsoleApp.Advanced.Configuration;

/// <summary>
/// HTTP Client configuration options
/// </summary>
public class HttpClientOptions
{
    /// <summary>
    /// Base URL for API requests
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.example.com";

    /// <summary>
    /// Request timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Number of retry attempts
    /// </summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Enable detailed logging for HTTP requests
    /// </summary>
    public bool EnableLogging { get; set; } = true;
}
