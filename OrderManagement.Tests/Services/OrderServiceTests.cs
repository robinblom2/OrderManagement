using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Dtos;
using OrderManagement.Api.Services;
using OrderManagement.Tests.Fixtures;

namespace OrderManagement.Tests.Services;

/// <summary>
/// Integration tests for OrderService using a PostgreSQL database
/// managed by Testcontainers.
/// </summary>
public class OrderServiceTests : IClassFixture<PostgresFixture>, IAsyncLifetime
{
    private readonly PostgresFixture _fixture;

    public OrderServiceTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await using var context = _fixture.CreateDbContext();
        await context.Orders.ExecuteDeleteAsync();
    }

    [Fact]
    public async Task GetOrderAsync_WhenOrderDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        await using var context = _fixture.CreateDbContext();
        var orderService = new OrderService(context);

        // Act
        var order = await orderService.GetOrderAsync(-1);

        // Assert
        Assert.Null(order);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenValidOrder_ShouldPersistOrder()
    {
        // Arrange
        await using var firstContext = _fixture.CreateDbContext();

        var orderService = new OrderService(firstContext);

        var createOrderDto = new CreateOrderDto() { CustomerName = "John Doe", TotalAmount = 144.44M };

        // Act
        var createdOrder = await orderService.CreateOrderAsync(createOrderDto);

        // Assert
        await using var secondContext = _fixture.CreateDbContext();
        var savedOrder = await secondContext.Orders.FindAsync(createdOrder.Id);

        Assert.True(createdOrder.Id > 0);
        Assert.NotNull(savedOrder);
        Assert.Equal(createdOrder.Id, savedOrder.Id);
        Assert.Equal(createOrderDto.CustomerName, savedOrder.CustomerName);
        Assert.Equal(createOrderDto.TotalAmount, savedOrder.TotalAmount);
    }

    [Fact]
    public async Task UpdateOrderAsync_WhenOrderExists_ShouldPersistChanges()
    {
        // Arrange
        await using var firstContext = _fixture.CreateDbContext();
        var orderService = new OrderService(firstContext);

        var createOrderDto = new CreateOrderDto() { CustomerName = "John Doe", TotalAmount = 144.44M };
        var order = await orderService.CreateOrderAsync(createOrderDto);

        var updateOrderDto = new UpdateOrderDto() { CustomerName = "Jane Doe", TotalAmount = 95.33M };

        // Act
        var result = await orderService.UpdateOrderAsync(order.Id, updateOrderDto);

        // Assert
        Assert.True(result);

        await using var secondContext = _fixture.CreateDbContext();
        var updatedOrder = await secondContext.Orders.FindAsync(order.Id);

        Assert.NotNull(updatedOrder);
        Assert.Equal(updateOrderDto.CustomerName, updatedOrder.CustomerName);
        Assert.Equal(updateOrderDto.TotalAmount, updatedOrder.TotalAmount);
    }

    [Fact]
    public async Task UpdateOrderAsync_WhenOrderDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        await using var context = _fixture.CreateDbContext();
        var orderService = new OrderService(context);

        var updateOrderDto = new UpdateOrderDto() { CustomerName = "John Doe", TotalAmount = 144.44M };

        // Act
        var result = await orderService.UpdateOrderAsync(-1, updateOrderDto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteOrderAsync_WhenOrderExists_ShouldRemoveOrder()
    {
        // Arrange
        await using var firstContext = _fixture.CreateDbContext();
        var orderService = new OrderService(firstContext);

        var createOrderDto = new CreateOrderDto() { CustomerName = "John Doe", TotalAmount = 144.44M };
        var createdOrder = await orderService.CreateOrderAsync(createOrderDto);

        // Act
        var result = await orderService.DeleteOrderAsync(createdOrder.Id);

        // Assert
        Assert.True(result);

        await using var secondContext = _fixture.CreateDbContext();
        var order = await secondContext.Orders.FindAsync(createdOrder.Id);

        Assert.Null(order);
    }

    [Fact]
    public async Task DeleteOrderAsync_WhenOrderDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        await using var context = _fixture.CreateDbContext();
        var orderService = new OrderService(context);

        // Act
        var result = await orderService.DeleteOrderAsync(-1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAllOrdersAsync_WhenNoOrdersExist_ShouldReturnEmptyList()
    {
        // Arrange
        await using var context = _fixture.CreateDbContext();
        var orderService = new OrderService(context);

        // Act
        var result = await orderService.GetAllOrdersAsync();

        // Assert
        Assert.Empty(result);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}