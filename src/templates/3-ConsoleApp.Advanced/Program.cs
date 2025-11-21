using ConsoleApp.Advanced.Extensions;
using ConsoleApp.Advanced.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
//#if (AddSerilog)
using Serilog;
//#endif
//#if (UseSystemCommandLine || UseSpectreConsole || UseCommandLineParser)
using ConsoleApp.Advanced;
//#endif

var builder = Host.CreateApplicationBuilder(args);

// Configure services using extension methods
builder.Services.AddApplicationServices();

//#if (UseDapper || UseEfCore)
// Add database services with Factory Pattern
builder.Services.AddDatabase(builder.Configuration);
//#endif

//#if (UseHttpClientBasic || UseHttpClientWithPolly)
// Add typed HttpClient with optional Polly resilience
builder.Services.AddApiClient(builder.Configuration);
//#endif

//#if (AddSerilog)
// Setup logging with Serilog
builder.Services.AddSerilog((services, lc) => lc
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services));
//#else
// Setup logging with console output
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
//#endif

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
