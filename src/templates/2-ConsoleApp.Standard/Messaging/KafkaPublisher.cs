//#if (UseKafka)
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Standard.Messaging;

public class KafkaPublisher : IMessagePublisher
{
    private readonly ILogger<KafkaPublisher> _logger;
    private readonly IProducer<Null, string> _producer;
    private readonly string _topic;

    public KafkaPublisher(ILogger<KafkaPublisher> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092"
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
        _topic = configuration["Kafka:Topic"] ?? "default-topic";
    }

    public async Task PublishAsync(MessageDto message, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(message);
        var kafkaMessage = new Message<Null, string> { Value = json };

        var result = await _producer.ProduceAsync(_topic, kafkaMessage, cancellationToken);
        _logger.LogInformation("Published message: {MessageId} to partition {Partition}", message.Id, result.Partition);
    }
}
//#endif
