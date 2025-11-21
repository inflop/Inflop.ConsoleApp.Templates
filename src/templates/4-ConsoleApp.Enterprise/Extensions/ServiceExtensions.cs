using ConsoleApp.Enterprise.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp.Enterprise.Extensions;

/// <summary>
/// Extension methods for configuring application services.
/// </summary>
/// <remarks>
/// This class demonstrates the extension method pattern for clean service registration.
/// Separating service configuration from Program.cs improves maintainability and follows
/// the Single Responsibility Principle.
/// </remarks>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds application services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    /// <returns>The service collection for method chaining</returns>
    /// <remarks>
    /// Register all application-specific services here. Group related services
    /// together and consider creating multiple extension methods for different
    /// feature areas in larger applications.
    /// </remarks>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register application services
        services.AddTransient<AppService>();
        services.AddHostedService<PrimaryWorker>();
        services.AddHostedService<SecondaryWorker>();

        return services;
    }
}
