using ConsoleApp.Advanced.Data.Models;
using ConsoleApp.Advanced.Infrastructure;
using Dapper;

namespace ConsoleApp.Advanced.Data;

/// <summary>
/// Dapper implementation of Example repository.
/// NOTE: This single implementation works with SQLite, SQL Server, AND PostgreSQL
/// thanks to the Factory Pattern - no code duplication!
/// </summary>
public class ExampleRepository : IExampleRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ExampleRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IEnumerable<ExampleEntity>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<ExampleEntity>(
            "SELECT Id, Name, CreatedAt FROM Examples ORDER BY CreatedAt DESC");
    }

    public async Task<ExampleEntity?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<ExampleEntity>(
            "SELECT Id, Name, CreatedAt FROM Examples WHERE Id = @Id",
            new { Id = id });
    }

    public async Task<int> AddAsync(ExampleEntity entity)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
            INSERT INTO Examples (Name, CreatedAt)
            VALUES (@Name, @CreatedAt);
            SELECT last_insert_rowid();"; // SQLite syntax - adjust for other DBs if needed

        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task<bool> UpdateAsync(ExampleEntity entity)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
            UPDATE Examples
            SET Name = @Name, CreatedAt = @CreatedAt
            WHERE Id = @Id";

        var rowsAffected = await connection.ExecuteAsync(sql, entity);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = "DELETE FROM Examples WHERE Id = @Id";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
}
