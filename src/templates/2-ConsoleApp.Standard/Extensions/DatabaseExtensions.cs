//#if (UseDapper || UseEfCore)
using ConsoleApp.Standard.Data;
//#if (UseDapper)
using ConsoleApp.Standard.Infrastructure;
//#endif
//#if (UseEfCore)
using Microsoft.EntityFrameworkCore;
//#endif
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp.Standard.Extensions;

/// <summary>
/// Extension methods for database service registration.
/// Demonstrates Factory Pattern - clean separation, no code duplication.
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Registers database services using Factory Pattern.
    /// Single method works for SQLite, SQL Server, AND PostgreSQL!
    /// </summary>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
//#if (UseDapper)
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' not found");

        var databaseType = configuration["Database:Type"] ?? "sqlite";

        // Factory Pattern: Register the appropriate connection factory based on configuration
        // This eliminates the need for separate repository implementations per database!
        services.AddSingleton<IDbConnectionFactory>(sp =>
        {
//#if (UseSqlite && UseSqlServer && UsePostgres)
            return databaseType.ToLowerInvariant() switch
            {
                "sqlite" => new SqliteConnectionFactory(connectionString),
                "sqlserver" => new SqlServerConnectionFactory(connectionString),
                "postgres" => new PostgresConnectionFactory(connectionString),
                _ => throw new InvalidOperationException($"Unsupported database type: {databaseType}")
            };
//#elif (UseSqlite && UseSqlServer)
            return databaseType.ToLowerInvariant() switch
            {
                "sqlite" => new SqliteConnectionFactory(connectionString),
                "sqlserver" => new SqlServerConnectionFactory(connectionString),
                _ => throw new InvalidOperationException($"Unsupported database type: {databaseType}")
            };
//#elif (UseSqlite && UsePostgres)
            return databaseType.ToLowerInvariant() switch
            {
                "sqlite" => new SqliteConnectionFactory(connectionString),
                "postgres" => new PostgresConnectionFactory(connectionString),
                _ => throw new InvalidOperationException($"Unsupported database type: {databaseType}")
            };
//#elif (UseSqlServer && UsePostgres)
            return databaseType.ToLowerInvariant() switch
            {
                "sqlserver" => new SqlServerConnectionFactory(connectionString),
                "postgres" => new PostgresConnectionFactory(connectionString),
                _ => throw new InvalidOperationException($"Unsupported database type: {databaseType}")
            };
//#elif (UseSqlite)
            return new SqliteConnectionFactory(connectionString);
//#elif (UseSqlServer)
            return new SqlServerConnectionFactory(connectionString);
//#elif (UsePostgres)
            return new PostgresConnectionFactory(connectionString);
//#endif
        });

        // Single repository implementation works with ALL database providers
        services.AddScoped<IExampleRepository, ExampleRepository>();
//#endif

//#if (UseEfCore)
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' not found");

//#if (UseSqlite)
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));
//#endif
//#if (UseSqlServer)
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));
//#endif
//#if (UsePostgres)
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
//#endif

        services.AddScoped<IExampleRepository, ExampleRepository>();
//#endif

        return services;
    }
}
//#endif
