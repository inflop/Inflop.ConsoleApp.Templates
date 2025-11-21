namespace ConsoleApp.Advanced.Services;

/// <summary>
/// Interface for API client demonstrating typed HttpClient pattern.
/// </summary>
public interface IApiClient
{
//#if (UseAsync)
    /// <summary>
    /// Gets data from the API endpoint asynchronously.
    /// </summary>
    Task<string> GetDataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Posts data to the API endpoint asynchronously.
    /// </summary>
    Task<bool> PostDataAsync(string data, CancellationToken cancellationToken = default);
//#else
    /// <summary>
    /// Gets data from the API endpoint.
    /// </summary>
    string GetData();

    /// <summary>
    /// Posts data to the API endpoint.
    /// </summary>
    bool PostData(string data);
//#endif
}
