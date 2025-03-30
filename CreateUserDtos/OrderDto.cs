using ECommerceAPI.Models;

namespace ECommerceAPI.CreateUserDtos
{
    public class OrderDto
    {
        public Decimal TotalAmount { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
