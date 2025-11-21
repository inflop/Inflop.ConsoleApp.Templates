//#if (UseRabbitMQ)
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ConsoleApp.Standard.Messaging;

/// <summary>
/// RabbitMQ message consumer implementation.
/// </summary>
public class RabbitMqConsumer : IMessageConsumer
{
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly IConfiguration _configuration;

    public RabbitMqConsumer(ILogger<RabbitMqConsumer> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task ConsumeAsync(Func<MessageDto, Task> onMessageReceived, CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
            Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672")
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        var queueName = _configuration["RabbitMQ:QueueName"] ?? "default-queue";
        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

        _logger.LogInformation("RabbitMQ consumer started, listening on queue: {QueueName}", queueName);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<MessageDto>(json);

                if (message != null)
                {
                    _logger.LogInformation("Received message: {MessageId}", message.Id);
                    await onMessageReceived(message);
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                channel.BasicNack(ea.DeliveryTag, false, true); // Requeue on error
            }
        };

        channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

        // Keep consuming until cancellation
        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("RabbitMQ consumer stopped");
        }
    }
}
//#endif
