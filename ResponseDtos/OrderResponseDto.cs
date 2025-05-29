using ECommerceAPI.Models;

namespace ECommerceAPI.ResponseDtos
{
    public class OrderResponseDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public int Phone { get; set; }
        public DateTime OrderDate { get; set; }
        
        public IEnumerable<OrderItemResponseDto> Items { get; set; } = new List<OrderItemResponseDto>();
    }
}
