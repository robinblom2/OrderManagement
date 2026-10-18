namespace OrderManagement.Api.Models;

public class Order
{
    public int Id { get; set; }
    public required string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
