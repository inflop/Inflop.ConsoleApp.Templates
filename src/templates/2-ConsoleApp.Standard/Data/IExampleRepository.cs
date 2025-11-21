using ConsoleApp.Standard.Data.Models;

namespace ConsoleApp.Standard.Data;

/// <summary>
/// Repository interface for Example entities.
/// </summary>
public interface IExampleRepository
{
    Task<IEnumerable<ExampleEntity>> GetAllAsync();
    Task<ExampleEntity?> GetByIdAsync(int id);
    Task<int> AddAsync(ExampleEntity entity);
    Task<bool> UpdateAsync(ExampleEntity entity);
    Task<bool> DeleteAsync(int id);
}
