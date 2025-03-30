namespace ECommerceAPI.CreateUserDtos
{
    public class OrderItemDto
    {
        public int Quantity { get; set; } 
        public decimal Price { get; set; } // Store price at the time of purchase
        public int OrderId { get; set; }
        public int ProductId { get; set; }
    }
}
