using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Data;
using OrderManagement.Api.Dtos;
using OrderManagement.Api.Models;

namespace OrderManagement.Api.Services;

public class OrderService(AppDbContext context)
{
    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await context.Orders.ToListAsync();
    }

    public async Task<Order?> GetOrderAsync(int id)
    {
        return await context.Orders.FindAsync(id);
    }

    public async Task<Order> CreateOrderAsync(CreateOrderDto dto)
    {
        var order = new Order 
        { 
            CustomerName = dto.CustomerName, 
            TotalAmount = dto.TotalAmount 
        };

        context.Orders.Add(order);

        await context.SaveChangesAsync();

        return order;
    }

    public async Task<bool> UpdateOrderAsync(int id, UpdateOrderDto dto)
    {
        var order = await GetOrderAsync(id);

        if (order == null)
        {
            return false;
        }

        order.CustomerName = dto.CustomerName;
        order.TotalAmount = dto.TotalAmount;

        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        var order = await GetOrderAsync(id);

        if (order == null)
        {
            return false;
        }

        context.Orders.Remove(order);

        await context.SaveChangesAsync();

        return true;
    }
}
