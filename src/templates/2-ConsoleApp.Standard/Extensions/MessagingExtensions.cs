//#if (UseRabbitMQ || UseAzureServiceBus || UseKafka)
using ConsoleApp.Standard.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp.Standard.Extensions;

/// <summary>
/// Extension methods for messaging service registration.
/// </summary>
public static class MessagingExtensions
{
    /// <summary>
    /// Registers messaging services (consumer and publisher).
    /// </summary>
    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
//#if (UseRabbitMQ)
        services.AddSingleton<IMessageConsumer, RabbitMqConsumer>();
        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
//#endif

//#if (UseAzureServiceBus)
        services.AddSingleton<IMessageConsumer, AzureServiceBusConsumer>();
        services.AddSingleton<IMessagePublisher, AzureServiceBusPublisher>();
//#endif

//#if (UseKafka)
        services.AddSingleton<IMessageConsumer, KafkaConsumer>();
        services.AddSingleton<IMessagePublisher, KafkaPublisher>();
//#endif

        return services;
    }
}
//#endif
