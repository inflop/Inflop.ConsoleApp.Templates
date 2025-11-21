//#if (UseAzureServiceBus)
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Simple.Messaging;

public class AzureServiceBusConsumer : IMessageConsumer
{
    private readonly ILogger<AzureServiceBusConsumer> _logger;
    private readonly IConfiguration _configuration;

    public AzureServiceBusConsumer(ILogger<AzureServiceBusConsumer> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task ConsumeAsync(Func<MessageDto, Task> onMessageReceived, CancellationToken cancellationToken)
    {
        var connectionString = _configuration["AzureServiceBus:ConnectionString"]
            ?? throw new InvalidOperationException("Azure Service Bus connection string not configured");
        var queueName = _configuration["AzureServiceBus:QueueName"] ?? "default-queue";

        await using var client = new ServiceBusClient(connectionString);
        await using var processor = client.CreateProcessor(queueName);

        processor.ProcessMessageAsync += async args =>
        {
            try
            {
                var body = args.Message.Body.ToString();
                var message = JsonSerializer.Deserialize<MessageDto>(body);

                if (message != null)
                {
                    _logger.LogInformation("Received message: {MessageId}", message.Id);
                    await onMessageReceived(message);
                    await args.CompleteMessageAsync(args.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
        };

        processor.ProcessErrorAsync += args =>
        {
            _logger.LogError(args.Exception, "Error in message processor");
            return Task.CompletedTask;
        };

        await processor.StartProcessingAsync(cancellationToken);
        _logger.LogInformation("Azure Service Bus consumer started");

        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            await processor.StopProcessingAsync();
            _logger.LogInformation("Azure Service Bus consumer stopped");
        }
    }
}
//#endif
