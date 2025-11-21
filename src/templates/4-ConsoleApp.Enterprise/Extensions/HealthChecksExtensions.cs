//#if (UseHealthChecksBasic || UseHealthChecksAspNet)
using ConsoleApp.Enterprise.Health;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp.Enterprise.Extensions;

/// <summary>
/// Extension methods for health checks registration.
/// </summary>
public static class HealthChecksExtensions
{
    /// <summary>
    /// Registers health checks with automatic checks for database, messaging, and HTTP clients.
    /// </summary>
    public static IServiceCollection AddHealthChecksWithDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var healthChecksBuilder = services.AddHealthChecks()
            .AddCheck<CustomHealthCheck>("custom");

//#if (UseSqlServer)
        // SQL Server health check
        healthChecksBuilder.AddSqlServer(
            configuration.GetConnectionString("Default")!,
            name: "database",
            tags: new[] { "db", "sql" });
//#endif

//#if (UsePostgres)
        // PostgreSQL health check
        healthChecksBuilder.AddNpgSql(
            configuration.GetConnectionString("Default")!,
            name: "database",
            tags: new[] { "db", "postgres" });
//#endif

//#if (UseSqlite)
        // SQLite health check
        healthChecksBuilder.AddSqlite(
            configuration.GetConnectionString("Default")!,
            name: "database",
            tags: new[] { "db", "sqlite" });
//#endif

//#if (UseHttpClientBasic || UseHttpClientWithPolly)
        // HTTP API health check
        var apiUrl = configuration["HttpClient:BaseUrl"];
        if (!string.IsNullOrEmpty(apiUrl))
        {
            healthChecksBuilder.AddUrlGroup(
                new Uri(apiUrl),
                name: "external-api",
                tags: new[] { "api" });
        }
//#endif

//#if (UseRabbitMQ)
        // RabbitMQ health check
        var rabbitHost = configuration["RabbitMQ:HostName"] ?? "localhost";
        var rabbitPort = configuration["RabbitMQ:Port"] ?? "5672";
        var rabbitConnectionString = $"amqp://{rabbitHost}:{rabbitPort}";

        healthChecksBuilder.AddRabbitMQ(
            rabbitConnectionString,
            name: "rabbitmq",
            tags: new[] { "messaging", "rabbitmq" });
//#endif

        return services;
    }
}
//#endif
