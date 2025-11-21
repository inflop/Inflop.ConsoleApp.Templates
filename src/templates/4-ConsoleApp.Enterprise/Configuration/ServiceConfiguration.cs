using ConsoleApp.Enterprise.Services;
using Microsoft.Extensions.DependencyInjection;
//#if (UseDapper || UseEfCore)
using ConsoleApp.Enterprise.Extensions;
using Microsoft.Extensions.Configuration;
//#endif

namespace ConsoleApp.Enterprise.Configuration;

/// <summary>
/// Centralized service configuration for dependency injection.
/// </summary>
/// <remarks>
/// This class demonstrates the service configuration pattern for organizing
/// DI registration. Separating service configuration from Program.cs improves
/// maintainability and follows the Single Responsibility Principle.
/// Group related services together and consider creating multiple configuration
/// classes for different feature areas in larger applications.
/// </remarks>
public static class ServiceConfiguration
{
    /// <summary>
    /// Configures all application services.
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    /// <remarks>
    /// Register services with appropriate lifetimes:
    /// - AddSingleton: Single instance for application lifetime
    /// - AddScoped: Instance per scope (not typically used in console apps)
    /// - AddTransient: New instance each time requested
    /// </remarks>
//#if (UseDapper || UseEfCore)
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Add database services with Factory Pattern
        services.AddDatabase(configuration);

        // Register application services
        services.AddTransient<IAppService, AppService>();
        services.AddTransient<IDataProcessor, DataProcessor>();

        // Register background workers
        services.AddHostedService<PrimaryWorker>();
        services.AddHostedService<SecondaryWorker>();
    }
//#else
    public static void ConfigureServices(IServiceCollection services)
    {
        // Register application services
        services.AddTransient<IAppService, AppService>();
        services.AddTransient<IDataProcessor, DataProcessor>();

        // Register background workers
        services.AddHostedService<PrimaryWorker>();
        services.AddHostedService<SecondaryWorker>();
    }
//#endif
}
