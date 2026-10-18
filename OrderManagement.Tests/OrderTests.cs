using OrderManagement.Api.Models;

namespace OrderManagement.Tests;

public class OrderTests
{
    [Fact]
    public void OrderStatus_ShouldDefaultToPending()
    {
        // Act
        var order = new Order { CustomerName = "John Doe" };

        // Assert
        Assert.Equal("Pending", order.Status);
    }
}
