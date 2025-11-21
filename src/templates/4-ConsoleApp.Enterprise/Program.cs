using ConsoleApp.Enterprise.Configuration;
using ConsoleApp.Enterprise.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
//#if (AddSerilog)
using Serilog;
//#endif
//#if (UseSystemCommandLine || UseSpectreConsole || UseCommandLineParser)
using ConsoleApp.Enterprise;
//#endif

var builder = Host.CreateApplicationBuilder(args);

// ============================================================================
// CONFIGURATION SETUP
// ============================================================================
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

// ============================================================================
// STRONGLY-TYPED CONFIGURATION
// ============================================================================
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<WorkerSettings>(builder.Configuration.GetSection("WorkerSettings"));

// ============================================================================
// LOGGING SETUP
// ============================================================================
//#if (AddSerilog)
builder.Services.AddSerilog((services, lc) => lc
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName));
//#else
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Configure log levels based on environment
if (builder.Environment.IsDevelopment())
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}
else
{
    builder.Logging.SetMinimumLevel(LogLevel.Information);
}
//#endif

// ============================================================================
// DEPENDENCY INJECTION SETUP
// ============================================================================
//#if (UseDapper || UseEfCore)
ServiceConfiguration.ConfigureServices(builder.Services, builder.Configuration);
//#else
ServiceConfiguration.ConfigureServices(builder.Services);
//#endif

// ============================================================================
// APPLICATION STARTUP
// ============================================================================
var app = builder.Build();

//#if (AddSerilog)
try
{
//#if (UseAsync)
    await app.RunAsync();
//#else
    app.Run();
//#endif
}
finally
{
    Log.CloseAndFlush();
}
//#else
//#if (UseAsync)
await app.RunAsync();
//#else
app.Run();
//#endif
//#endif
