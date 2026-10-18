using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Data;
using Testcontainers.PostgreSql;

namespace OrderManagement.Tests.Fixtures;

/// <summary>
///  The fixture is responsible for preparing our testing environment.
///  It needs a DbContext to apply database migrations before tests execute.
///  Once migrations have been applied, this context is no longer needed.
/// </summary>
public class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;
    public string ConnectionString => _container.GetConnectionString();

    public PostgresFixture()
    {
        // Configure the container without starting it
        _container = new PostgreSqlBuilder("postgres:17")
            .WithDatabase("ordermanagement_test")
            .WithUsername("testuser")
            .WithPassword("testpassword")
            .Build();
    }

    /// <summary>
    /// Starts PostgreSQL, creates a temporary DbContext, applies migrations, and disposes of the context.
    /// </summary>
    /// <returns></returns>
    public async Task InitializeAsync()
    {
        // Start PostgreSQL container
        await _container.StartAsync();

        // Create a temporary context
        await using var context = CreateDbContext();

        // Apply migrations to DB
        await context.Database.MigrateAsync();
    }

    /// <summary>
    /// Disposes of the PostgreSQL container.
    /// </summary>
    /// <returns></returns>
    public async Task DisposeAsync()
    {
        // Stop and dispose of PostgreSQL container
        await _container.DisposeAsync();
    }

    /// <summary>
    /// Creates a fresh context whenever a test needs one.
    /// </summary>
    /// <returns></returns>
    public AppDbContext CreateDbContext()
    {
        // Create and return a DbContext
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString).Options;

        return new AppDbContext(options);
    }
}
