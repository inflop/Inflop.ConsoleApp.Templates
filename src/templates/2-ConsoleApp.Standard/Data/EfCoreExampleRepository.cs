using ConsoleApp.Standard.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp.Standard.Data;

/// <summary>
/// Entity Framework Core implementation of Example repository.
/// </summary>
public class ExampleRepository : IExampleRepository
{
    private readonly AppDbContext _context;

    public ExampleRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<ExampleEntity>> GetAllAsync()
    {
        return await _context.Examples
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<ExampleEntity?> GetByIdAsync(int id)
    {
        return await _context.Examples
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<int> AddAsync(ExampleEntity entity)
    {
        _context.Examples.Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(ExampleEntity entity)
    {
        _context.Examples.Update(entity);
        var rowsAffected = await _context.SaveChangesAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Examples.FindAsync(id);
        if (entity == null)
            return false;

        _context.Examples.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
