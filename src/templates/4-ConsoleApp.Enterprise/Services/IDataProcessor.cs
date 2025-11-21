namespace ConsoleApp.Enterprise.Services;

/// <summary>
/// Interface for data processing operations.
/// </summary>
/// <remarks>
/// This interface demonstrates the interface segregation principle.
/// Keep interfaces small and focused on specific responsibilities.
/// </remarks>
public interface IDataProcessor
{
//#if (UseAsync)
    /// <summary>
    /// Processes data asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for operation cancellation</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task ProcessAsync(CancellationToken cancellationToken = default);
//#else
    /// <summary>
    /// Processes data.
    /// </summary>
    void Process();
//#endif
}
