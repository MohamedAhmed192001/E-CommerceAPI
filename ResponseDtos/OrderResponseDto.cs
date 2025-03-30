using ECommerceAPI.Models;

namespace ECommerceAPI.ResponseDtos
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public Decimal TotalAmount { get; set; }
        public string UserEmail { get; set; }
        public IEnumerable<OrderItemResponseDto> OrderItems { get; set; } = new List<OrderItemResponseDto>();
    }
}
