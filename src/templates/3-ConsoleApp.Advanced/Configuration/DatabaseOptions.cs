namespace ConsoleApp.Advanced.Configuration;

/// <summary>
/// Database configuration options
/// </summary>
public class DatabaseOptions
{
    /// <summary>
    /// Database connection string
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Database type (sqlite, sqlserver, postgres)
    /// </summary>
    public string Type { get; set; } = "sqlite";

    /// <summary>
    /// Command timeout in seconds
    /// </summary>
    public int CommandTimeout { get; set; } = 30;

    /// <summary>
    /// Enable detailed logging for database operations
    /// </summary>
    public bool EnableDetailedErrors { get; set; } = false;
}
