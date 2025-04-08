namespace ECommerceAPI.PaymentDTOs
{
    // using this dto when the customer starts in payment process
    public class CheckoutDto
    {
        public int OrderId { get; set; }
        public string Currency { get; set; } = "usd";
    }
}
