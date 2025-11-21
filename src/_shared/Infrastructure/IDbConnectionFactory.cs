using System.Data;

namespace ConsoleApp.Advanced.Infrastructure;

/// <summary>
/// Factory interface for creating database connections.
/// Allows switching between different database providers without changing repository code.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates a new database connection for the configured provider.
    /// </summary>
    /// <returns>An open database connection</returns>
    IDbConnection CreateConnection();
}
