using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Data;
using Testcontainers.PostgreSql;

namespace OrderManagement.Tests.Fixtures;

/// <summary>
/// Manages a PostgreSQL Testcontainer and applies database migrations
/// before integration tests execute.
/// </summary>
public class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;
    public string ConnectionString => _container.GetConnectionString();

    public PostgresFixture()
    {
        _container = new PostgreSqlBuilder("postgres:17")
            .WithDatabase("ordermanagement_test")
            .WithUsername("testuser")
            .WithPassword("testpassword")
            .Build();
    }

    /// <summary>
    /// Starts the test database and applies EF Core migrations.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using var context = CreateDbContext();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    public AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString).Options;

        return new AppDbContext(options);
    }
}
