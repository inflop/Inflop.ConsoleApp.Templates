using ConsoleApp.Standard.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
//#if (AddSerilog)
using Serilog;
//#endif
//#if (UseSystemCommandLine || UseSpectreConsole || UseCommandLineParser)
using ConsoleApp.Standard;
//#endif
//#if (UseDapper || UseEfCore || UseSystemCommandLine || UseSpectreConsole || UseCommandLineParser || UseHttpClientBasic || UseHttpClientWithPolly || UseHealthChecksBasic || UseHealthChecksAspNet || UseRabbitMQ || UseAzureServiceBus || UseKafka)
using ConsoleApp.Standard.Extensions;
//#endif

var builder = Host.CreateApplicationBuilder(args);

// ============================================================================
// CONFIGURATION SETUP
// ============================================================================
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

// ============================================================================
// LOGGING SETUP
// ============================================================================
//#if (AddSerilog)
builder.Services.AddSerilog((services, lc) => lc
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services));
//#else
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);
//#endif

// ============================================================================
// DEPENDENCY INJECTION SETUP
// ============================================================================
//#if (UseDapper || UseEfCore)
// Add database services with Factory Pattern
builder.Services.AddDatabase(builder.Configuration);
//#endif

builder.Services.AddTransient<AppService>();
builder.Services.AddHostedService<AppBackgroundService>();

// ============================================================================
// APPLICATION STARTUP
// ============================================================================
var app = builder.Build();

//#if (AddSerilog)
try
{
    await app.RunAsync();
}
finally
{
    Log.CloseAndFlush();
}
//#else
await app.RunAsync();
//#endif
