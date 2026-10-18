using OrderManagement.Api.Models;

namespace OrderManagement.Tests
{
    /// <summary>
    /// This class contains unit tests written with Xunit.
    /// </summary>
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
}
