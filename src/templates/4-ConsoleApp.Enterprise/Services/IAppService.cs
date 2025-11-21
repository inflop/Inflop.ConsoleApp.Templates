namespace ConsoleApp.Enterprise.Services;

/// <summary>
/// Interface for the main application service.
/// </summary>
/// <remarks>
/// This interface demonstrates the dependency inversion principle.
/// Clients depend on abstractions rather than concrete implementations,
/// enabling testability, flexibility, and adherence to SOLID principles.
/// </remarks>
public interface IAppService
{
//#if (UseAsync)
    /// <summary>
    /// Executes the main application logic asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for operation cancellation</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task ExecuteAsync(CancellationToken cancellationToken = default);
//#else
    /// <summary>
    /// Executes the main application logic.
    /// </summary>
    void Execute();
//#endif
}
