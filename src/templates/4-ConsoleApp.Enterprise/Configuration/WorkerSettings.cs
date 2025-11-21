namespace ConsoleApp.Enterprise.Configuration;

/// <summary>
/// Configuration settings for background workers.
/// </summary>
/// <remarks>
/// This class demonstrates configuration for background workers including
/// timing intervals and execution modes. Use this pattern for configurable
/// worker behavior without code changes.
/// </remarks>
public class WorkerSettings
{
    /// <summary>
    /// Gets or sets the worker execution interval in milliseconds.
    /// </summary>
    public int ExecutionIntervalMs { get; set; } = 5000;

    /// <summary>
    /// Gets or sets a value indicating whether the worker should run continuously.
    /// </summary>
    public bool ContinuousExecution { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum number of worker iterations (0 = unlimited).
    /// </summary>
    public int MaxIterations { get; set; } = 0;
}
