using Microsoft.AspNetCore.Mvc;
using OrderManagement.Api.Dtos;
using OrderManagement.Api.Models;
using OrderManagement.Api.Services;

namespace OrderManagement.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(OrderService orderService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Order>>> GetAllOrdersAsync()
    {
        var orders = await orderService.GetAllOrdersAsync();

        return Ok(orders);
    }

    [HttpGet("{id:int}", Name = "GetOrderById")]
    public async Task<ActionResult<Order>> GetOrderAsync(int id)
    {
        var order = await orderService.GetOrderAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrderAsync([FromBody] CreateOrderDto dto)
    {
        var order = await orderService.CreateOrderAsync(dto);

        return CreatedAtRoute(
            routeName: "GetOrderById",
            routeValues: new { id = order.Id },
            value: order
        );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateOrderAsync(int id, [FromBody] UpdateOrderDto dto)
    {
        var updated = await orderService.UpdateOrderAsync(id, dto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOrderAsync(int id)
    {
        var deleted = await orderService.DeleteOrderAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
