using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Advanced.Services;

/// <summary>
/// Typed HttpClient implementation for API communication.
/// Demonstrates dependency injection of HttpClient and automatic configuration.
/// </summary>
public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient> _logger;

    public ApiClient(HttpClient httpClient, ILogger<ApiClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

//#if (UseAsync)
    public async Task<string> GetDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching data from API...");

            var response = await _httpClient.GetAsync("posts/1", cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("Successfully fetched data from API");

            return content;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed");
            throw;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Request was cancelled");
            throw;
        }
    }

    public async Task<bool> PostDataAsync(string data, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Posting data to API...");

            var content = new StringContent(
                JsonSerializer.Serialize(new { title = data, body = "Sample post body", userId = 1 }),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("posts", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            _logger.LogInformation("Successfully posted data to API");
            return true;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed");
            return false;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Request was cancelled");
            return false;
        }
    }
//#else
    // WARNING: Synchronous HTTP calls block threads and may cause performance issues
    // Consider using async/await pattern for production code
    public string GetData()
    {
        try
        {
            _logger.LogInformation("Fetching data from API...");

            var response = _httpClient.GetAsync("posts/1").GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            _logger.LogInformation("Successfully fetched data from API");

            return content;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed");
            throw;
        }
    }

    public bool PostData(string data)
    {
        try
        {
            _logger.LogInformation("Posting data to API...");

            var content = new StringContent(
                JsonSerializer.Serialize(new { title = data, body = "Sample post body", userId = 1 }),
                Encoding.UTF8,
                "application/json");

            var response = _httpClient.PostAsync("posts", content).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            _logger.LogInformation("Successfully posted data to API");
            return true;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed");
            return false;
        }
    }
//#endif
}
