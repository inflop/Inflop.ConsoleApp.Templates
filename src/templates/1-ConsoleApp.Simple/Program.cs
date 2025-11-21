using ConsoleApp.Simple.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
//#if (AddSerilog)
using Serilog;
//#endif
//#if (UseSystemCommandLine || UseSpectreConsole || UseCommandLineParser)
using ConsoleApp.Simple;
//#endif
//#if (UseDapper)
using ConsoleApp.Simple.Data;
using ConsoleApp.Simple.Infrastructure;
//#endif
//#if (UseEfCore)
using ConsoleApp.Simple.Data;
using Microsoft.EntityFrameworkCore;
//#endif
//#if (UseHealthChecksBasic || UseHealthChecksAspNet)
using ConsoleApp.Simple.Health;
//#endif
//#if (UseRabbitMQ || UseAzureServiceBus || UseKafka)
using ConsoleApp.Simple.Messaging;
//#endif
//#if (UseHttpClientWithPolly)
using Polly;
using Polly.Extensions.Http;
//#endif
//#if (UseSystemCommandLine)
using System.CommandLine;
//#endif
//#if (UseCommandLineParser)
using CommandLine;
//#endif
//#if (UseSpectreConsole)
using Spectre.Console;
//#endif

// ============================================================================
// CONFIGURATION SETUP
// ============================================================================
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

// ============================================================================
// DEPENDENCY INJECTION SETUP
// ============================================================================
var services = new ServiceCollection();

//#if (AddSerilog)
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

services.AddLogging(builder =>
{
    builder.ClearProviders();
    builder.AddSerilog(dispose: true);
});
//#else
// Add logging
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});
//#endif

// Add configuration
services.AddSingleton<IConfiguration>(configuration);

//#if (UseDapper)
// Add database services with Factory Pattern
var connectionString = configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' not found");

var databaseType = configuration["Database:Type"] ?? "sqlite";

services.AddSingleton<IDbConnectionFactory>(sp =>
{
//#if (UseSqlite && UseSqlServer && UsePostgres)
    return databaseType.ToLowerInvariant() switch
    {
        "sqlite" => new SqliteConnectionFactory(connectionString),
        "sqlserver" => new SqlServerConnectionFactory(connectionString),
        "postgres" => new PostgresConnectionFactory(connectionString),
        _ => throw new InvalidOperationException($"Unsupported database type: {databaseType}")
    };
//#elif (UseSqlite && UseSqlServer)
    return databaseType.ToLowerInvariant() switch
    {
        "sqlite" => new SqliteConnectionFactory(connectionString),
        "sqlserver" => new SqlServerConnectionFactory(connectionString),
        _ => throw new InvalidOperationException($"Unsupported database type: {databaseType}")
    };
//#elif (UseSqlite && UsePostgres)
    return databaseType.ToLowerInvariant() switch
    {
        "sqlite" => new SqliteConnectionFactory(connectionString),
        "postgres" => new PostgresConnectionFactory(connectionString),
        _ => throw new InvalidOperationException($"Unsupported database type: {databaseType}")
    };
//#elif (UseSqlServer && UsePostgres)
    return databaseType.ToLowerInvariant() switch
    {
        "sqlserver" => new SqlServerConnectionFactory(connectionString),
        "postgres" => new PostgresConnectionFactory(connectionString),
        _ => throw new InvalidOperationException($"Unsupported database type: {databaseType}")
    };
//#elif (UseSqlite)
    return new SqliteConnectionFactory(connectionString);
//#elif (UseSqlServer)
    return new SqlServerConnectionFactory(connectionString);
//#elif (UsePostgres)
    return new PostgresConnectionFactory(connectionString);
//#endif
});

services.AddScoped<IExampleRepository, ExampleRepository>();
//#endif

//#if (UseEfCore)
// Add Entity Framework Core DbContext
var connectionString = configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' not found");

//#if (UseSqlite)
services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
//#endif
//#if (UseSqlServer)
services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
//#endif
//#if (UsePostgres)
services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
//#endif

services.AddScoped<IExampleRepository, ExampleRepository>();
//#endif

//#if (UseHealthChecksBasic || UseHealthChecksAspNet)
// Add health checks with automatic dependency checks
var healthChecksBuilder = services.AddHealthChecks()
    .AddCheck<CustomHealthCheck>("custom");

//#if (UseSqlServer)
healthChecksBuilder.AddSqlServer(
    configuration.GetConnectionString("Default")!,
    name: "database",
    tags: new[] { "db", "sql" });
//#endif

//#if (UsePostgres)
healthChecksBuilder.AddNpgSql(
    configuration.GetConnectionString("Default")!,
    name: "database",
    tags: new[] { "db", "postgres" });
//#endif

//#if (UseSqlite)
healthChecksBuilder.AddSqlite(
    configuration.GetConnectionString("Default")!,
    name: "database",
    tags: new[] { "db", "sqlite" });
//#endif

//#if (UseHttpClientBasic || UseHttpClientWithPolly)
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
var rabbitHost = configuration["RabbitMQ:HostName"] ?? "localhost";
var rabbitPort = configuration["RabbitMQ:Port"] ?? "5672";
var rabbitConnectionString = $"amqp://{rabbitHost}:{rabbitPort}";

healthChecksBuilder.AddRabbitMQ(
    rabbitConnectionString,
    name: "rabbitmq",
    tags: new[] { "messaging", "rabbitmq" });
//#endif
//#endif

//#if (UseRabbitMQ)
// Add RabbitMQ messaging services
services.AddSingleton<IMessageConsumer, RabbitMqConsumer>();
services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
//#endif

//#if (UseAzureServiceBus)
// Add Azure Service Bus messaging services
services.AddSingleton<IMessageConsumer, AzureServiceBusConsumer>();
services.AddSingleton<IMessagePublisher, AzureServiceBusPublisher>();
//#endif

//#if (UseKafka)
// Add Kafka messaging services
services.AddSingleton<IMessageConsumer, KafkaConsumer>();
services.AddSingleton<IMessagePublisher, KafkaPublisher>();
//#endif

//#if (UseHttpClientBasic || UseHttpClientWithPolly)
// Add typed HttpClient with optional Polly resilience
var baseUrl = configuration["HttpClient:BaseUrl"] ?? "https://api.example.com";
var timeoutSeconds = int.Parse(configuration["HttpClient:TimeoutSeconds"] ?? "30");

//#if (UseHttpClientBasic)
services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
});
//#endif

//#if (UseHttpClientWithPolly)
var retryCount = int.Parse(configuration["HttpClient:RetryCount"] ?? "3");

services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
})
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(
        retryCount,
        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        onRetry: (outcome, timespan, retryCount, context) =>
        {
            // Log retry attempts if needed
        }))
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 5,
        durationOfBreak: TimeSpan.FromSeconds(30)));
//#endif
//#endif

// Register application services
services.AddTransient<MyService>();

// Build service provider
var serviceProvider = services.BuildServiceProvider();

// ============================================================================
// APPLICATION EXECUTION
// ============================================================================
try
{
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Application starting...");

//#if (UseSystemCommandLine)
    // Parse command-line arguments with System.CommandLine
    var cmdOptions = new CommandLineOptions();
    var rootCommand = new RootCommand("Console application with command-line arguments");

    var nameOption = new Option<string>(
        aliases: new[] { "--name", "-n" },
        description: "Your name",
        getDefaultValue: () => "World");

    var verboseOption = new Option<bool>(
        aliases: new[] { "--verbose", "-v" },
        description: "Enable verbose output");

    rootCommand.AddOption(nameOption);
    rootCommand.AddOption(verboseOption);

    rootCommand.SetHandler((name, verbose) =>
    {
        cmdOptions.Name = name;
        cmdOptions.Verbose = verbose;
    }, nameOption, verboseOption);

    rootCommand.Invoke(args);

    logger.LogInformation("Parsed command-line arguments: Name={Name}, Verbose={Verbose}",
        cmdOptions.Name, cmdOptions.Verbose);

    if (cmdOptions.Verbose)
    {
        logger.LogInformation("Verbose mode enabled");
    }
//#endif

//#if (UseSpectreConsole)
    // Parse command-line arguments with Spectre.Console
    var cmdOptions = new CommandLineOptions();

    var settings = AnsiConsole.Prompt(
        new TextPrompt<string>("Enter your name:")
            .DefaultValue("World"));
    cmdOptions.Name = settings;

    cmdOptions.Verbose = AnsiConsole.Confirm("Enable verbose output?");

    logger.LogInformation("Parsed command-line arguments: Name={Name}, Verbose={Verbose}",
        cmdOptions.Name, cmdOptions.Verbose);

    if (cmdOptions.Verbose)
    {
        logger.LogInformation("Verbose mode enabled");
    }
//#endif

//#if (UseCommandLineParser)
    // Parse command-line arguments with CommandLineParser
    var cmdOptions = new CommandLineOptions();

    Parser.Default.ParseArguments<CommandLineOptions>(args)
        .WithParsed(options =>
        {
            cmdOptions = options;
            logger.LogInformation("Parsed command-line arguments: Name={Name}, Verbose={Verbose}",
                cmdOptions.Name, cmdOptions.Verbose);

            if (cmdOptions.Verbose)
            {
                logger.LogInformation("Verbose mode enabled");
            }
        })
        .WithNotParsed(errors =>
        {
            logger.LogError("Failed to parse command-line arguments");
        });
//#endif

    var myService = serviceProvider.GetRequiredService<MyService>();
//#if (UseAsync)
    await myService.RunAsync();
//#else
    myService.Run();
//#endif

    logger.LogInformation("Application completed successfully");
}
catch (Exception ex)
{
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during application execution");
    return 1;
}
finally
{
    // Dispose service provider to release resources
    if (serviceProvider is IDisposable disposable)
    {
        disposable.Dispose();
    }
//#if (AddSerilog)
    // Ensure Serilog flushes any buffered log events
    Log.CloseAndFlush();
//#endif
}

return 0;
