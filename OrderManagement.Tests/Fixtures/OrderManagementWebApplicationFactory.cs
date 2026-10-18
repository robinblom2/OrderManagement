using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderManagement.Api.Data;

namespace OrderManagement.Tests.Fixtures;

public class OrderManagementWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly PostgresFixture _fixture;

    public OrderManagementWebApplicationFactory(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {

        builder.UseSetting("ConnectionStrings:DefaultConnection", _fixture.ConnectionString);

        // Ensure HTTP tests use the isolated PostgreSQL test database.
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();

            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(_fixture.ConnectionString));
        });   
    }
}
