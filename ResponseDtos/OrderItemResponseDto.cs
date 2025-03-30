using ECommerceAPI.Models;

namespace ECommerceAPI.ResponseDtos
{
    public class OrderItemResponseDto
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; } // Store price at the time of purchase
        public decimal Price { get; set; }
    }
}
