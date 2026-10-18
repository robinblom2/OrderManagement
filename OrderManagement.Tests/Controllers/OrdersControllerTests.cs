using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Dtos;
using OrderManagement.Api.Models;
using OrderManagement.Tests.Fixtures;
using System.Net;
using System.Net.Http.Json;

namespace OrderManagement.Tests.Controllers;

public class OrdersControllerTests : IClassFixture<PostgresFixture>, IAsyncLifetime
{
    private readonly PostgresFixture _fixture;

    public OrdersControllerTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await using var context = _fixture.CreateDbContext();

        await context.Orders.ExecuteDeleteAsync();
    }

    [Fact]
    public async Task GetAllOrders_WhenNoOrdersExist_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        await using var factory = new OrderManagementWebApplicationFactory(_fixture);
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/orders");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var orders = await response.Content.ReadFromJsonAsync<List<Order>>();

        Assert.NotNull(orders);
        Assert.Empty(orders);
    }

    [Fact]
    public async Task CreateOrder_WhenValid_ShouldReturnCreatedAndPersistOrder()
    {
        // Arrange
        var createOrderDto = new CreateOrderDto() { CustomerName = "John Doe", TotalAmount = 149.99M };

        await using var factory = new OrderManagementWebApplicationFactory(_fixture);
        using var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/orders", createOrderDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdOrder = await response.Content.ReadFromJsonAsync<Order>();

        Assert.NotNull(createdOrder);
        Assert.NotNull(response.Headers.Location);
        Assert.EndsWith($"/api/orders/{createdOrder.Id}", response.Headers.Location.ToString());
        Assert.True(createdOrder.Id > 0);
        Assert.Equal(createOrderDto.CustomerName, createdOrder.CustomerName);
        Assert.Equal(createOrderDto.TotalAmount, createdOrder.TotalAmount);

        await using var context = _fixture.CreateDbContext();
        var result = await context.Orders.SingleAsync(order => order.Id == createdOrder.Id);

        Assert.Equal(createOrderDto.CustomerName, result.CustomerName);
        Assert.Equal(createOrderDto.TotalAmount, result.TotalAmount);
    }

    [Fact]
    public async Task CreateOrder_WhenTotalAmountIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        var createOrderDto = new CreateOrderDto() { CustomerName = "John Doe", TotalAmount = 0 };

        await using var factory = new OrderManagementWebApplicationFactory(_fixture);
        using var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/orders", createOrderDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await using var context = _fixture.CreateDbContext();
        var orderExists = await context.Orders.AnyAsync();

        Assert.False(orderExists);
    }

    [Fact]
    public async Task GetOrder_WhenOrderDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        await using var factory = new OrderManagementWebApplicationFactory(_fixture);
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/orders/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateOrder_WhenOrderExists_ShouldReturnNoContentAndPersistChanges()
    {
        // Arrange
        await using var firstContext = _fixture.CreateDbContext();

        var order = new Order() { CustomerName = "John Doe", TotalAmount = 149.99M };
        firstContext.Orders.Add(order);
        await firstContext.SaveChangesAsync();

        var updatedOrderDto = new UpdateOrderDto() { CustomerName = "Jane Doe", TotalAmount = 199.99M };

        await using var factory = new OrderManagementWebApplicationFactory(_fixture);
        using var client = factory.CreateClient();

        // Act
        var response = await client.PutAsJsonAsync($"/api/orders/{order.Id}", updatedOrderDto);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        await using var secondContext = _fixture.CreateDbContext();
        var updatedOrder = await secondContext.Orders.FindAsync(order.Id);

        Assert.NotNull(updatedOrder);
        Assert.Equal(updatedOrderDto.CustomerName, updatedOrder.CustomerName);
        Assert.Equal(updatedOrderDto.TotalAmount, updatedOrder.TotalAmount);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
