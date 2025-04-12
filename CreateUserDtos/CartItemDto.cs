using Microsoft.EntityFrameworkCore.Metadata;

namespace ECommerceAPI.CreateUserDtos
{
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
