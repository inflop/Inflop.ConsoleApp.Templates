//#if (UseHttpClientBasic || UseHttpClientWithPolly)
using ConsoleApp.Advanced.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
//#if (UseHttpClientWithPolly)
using Polly;
using Polly.Extensions.Http;
//#endif

namespace ConsoleApp.Advanced.Extensions;

/// <summary>
/// Extension methods for HttpClient service registration.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Registers typed HttpClient with optional Polly resilience policies.
    /// </summary>
    public static IServiceCollection AddApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        var baseUrl = configuration["HttpClient:BaseUrl"] ?? "https://jsonplaceholder.typicode.com";
        var timeoutSeconds = int.Parse(configuration["HttpClient:TimeoutSeconds"] ?? "30");

//#if (UseHttpClientBasic)
        // Basic HttpClient configuration
        services.AddHttpClient<IApiClient, ApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        });
//#endif

//#if (UseHttpClientWithPolly)
        // HttpClient with Polly resilience policies
        var retryCount = int.Parse(configuration["HttpClient:RetryCount"] ?? "3");

        services.AddHttpClient<IApiClient, ApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        })
        // Retry policy for transient failures
        .AddPolicyHandler(HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry {retryCount} after {timespan.TotalSeconds}s delay");
                }))
        // Circuit breaker policy
        .AddPolicyHandler(HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (result, duration) =>
                {
                    Console.WriteLine($"Circuit breaker opened for {duration.TotalSeconds}s");
                },
                onReset: () =>
                {
                    Console.WriteLine("Circuit breaker reset");
                }))
        // Timeout policy
        .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(timeoutSeconds)));
//#endif

        return services;
    }
}
//#endif
