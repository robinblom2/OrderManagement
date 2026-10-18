using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Api.Dtos
{
    public class CreateOrderDto
    {
        [Required]
        public required string CustomerName { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal TotalAmount { get; set; }
    }
}
