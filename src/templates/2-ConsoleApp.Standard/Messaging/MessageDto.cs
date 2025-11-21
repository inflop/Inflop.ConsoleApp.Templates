//#if (UseRabbitMQ || UseAzureServiceBus || UseKafka)
namespace ConsoleApp.Standard.Messaging;

/// <summary>
/// Message data transfer object.
/// </summary>
public class MessageDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
//#endif
