namespace ECommerceAPI.CreateUserDtos
{
    public class StripeItemDto
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
