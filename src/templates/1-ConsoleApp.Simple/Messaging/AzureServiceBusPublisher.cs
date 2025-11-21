//#if (UseAzureServiceBus)
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Simple.Messaging;

public class AzureServiceBusPublisher : IMessagePublisher
{
    private readonly ILogger<AzureServiceBusPublisher> _logger;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;

    public AzureServiceBusPublisher(ILogger<AzureServiceBusPublisher> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var connectionString = configuration["AzureServiceBus:ConnectionString"]
            ?? throw new InvalidOperationException("Azure Service Bus connection string not configured");
        var queueName = configuration["AzureServiceBus:QueueName"] ?? "default-queue";

        _client = new ServiceBusClient(connectionString);
        _sender = _client.CreateSender(queueName);
    }

    public async Task PublishAsync(MessageDto message, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(message);
        var serviceBusMessage = new ServiceBusMessage(json);

        await _sender.SendMessageAsync(serviceBusMessage, cancellationToken);
        _logger.LogInformation("Published message: {MessageId}", message.Id);
    }
}
//#endif
