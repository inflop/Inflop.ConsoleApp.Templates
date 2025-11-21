namespace ConsoleApp.Advanced.Configuration;

/// <summary>
/// Messaging configuration options
/// </summary>
public class MessagingOptions
{
    /// <summary>
    /// Messaging provider (rabbitmq, azureservicebus, kafka)
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Connection string or host name
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Queue name or topic
    /// </summary>
    public string QueueName { get; set; } = string.Empty;

    /// <summary>
    /// Enable auto-acknowledgment
    /// </summary>
    public bool AutoAck { get; set; } = false;

    /// <summary>
    /// Maximum retry attempts for failed messages
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;
}
