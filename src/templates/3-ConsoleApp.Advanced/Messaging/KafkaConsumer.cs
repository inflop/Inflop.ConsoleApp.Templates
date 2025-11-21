//#if (UseKafka)
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Advanced.Messaging;

public class KafkaConsumer : IMessageConsumer
{
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly IConfiguration _configuration;

    public KafkaConsumer(ILogger<KafkaConsumer> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task ConsumeAsync(Func<MessageDto, Task> onMessageReceived, CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            GroupId = _configuration["Kafka:GroupId"] ?? "default-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        var topic = _configuration["Kafka:Topic"] ?? "default-topic";
        consumer.Subscribe(topic);

        _logger.LogInformation("Kafka consumer started, listening on topic: {Topic}", topic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(cancellationToken);
                    var message = JsonSerializer.Deserialize<MessageDto>(result.Message.Value);

                    if (message != null)
                    {
                        _logger.LogInformation("Received message: {MessageId}", message.Id);
                        await onMessageReceived(message);
                        consumer.Commit(result);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message");
                }
            }
        }
        catch (OperationCanceledException)
        {
            consumer.Close();
            _logger.LogInformation("Kafka consumer stopped");
        }
    }
}
//#endif
