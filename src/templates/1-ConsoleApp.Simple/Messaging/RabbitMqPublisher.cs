//#if (UseRabbitMQ)
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace ConsoleApp.Simple.Messaging;

/// <summary>
/// RabbitMQ message publisher implementation.
/// </summary>
public class RabbitMqPublisher : IMessagePublisher
{
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqPublisher(ILogger<RabbitMqPublisher> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
            Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672")
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        var queueName = _configuration["RabbitMQ:QueueName"] ?? "default-queue";
        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
    }

    public Task PublishAsync(MessageDto message, CancellationToken cancellationToken = default)
    {
        var queueName = _configuration["RabbitMQ:QueueName"] ?? "default-queue";
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        _channel.BasicPublish(
            exchange: string.Empty,
            routingKey: queueName,
            basicProperties: properties,
            body: body);

        _logger.LogInformation("Published message: {MessageId}", message.Id);
        return Task.CompletedTask;
    }
}
//#endif
