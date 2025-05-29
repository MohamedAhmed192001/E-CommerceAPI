using ECommerceAPI.Models;

namespace ECommerceAPI.CreateUserDtos
{
    public class OrderDto
    {
        public int UserId { get; set; }
        public int Phone { get; set; }
        public string Address { get; set; }
        public ICollection<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}
