//#if (UseRabbitMQ || UseAzureServiceBus || UseKafka)
namespace ConsoleApp.Standard.Messaging;

/// <summary>
/// Interface for message consumers.
/// </summary>
public interface IMessageConsumer
{
    /// <summary>
    /// Start consuming messages.
    /// </summary>
    Task ConsumeAsync(Func<MessageDto, Task> onMessageReceived, CancellationToken cancellationToken);
}
//#endif
