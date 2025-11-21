//#if (UseRabbitMQ || UseAzureServiceBus || UseKafka)
namespace ConsoleApp.Advanced.Messaging;

/// <summary>
/// Interface for message publishers.
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Publish a message.
    /// </summary>
    Task PublishAsync(MessageDto message, CancellationToken cancellationToken = default);
}
//#endif
