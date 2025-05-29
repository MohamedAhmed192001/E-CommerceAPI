using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public Decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string? PaymentIntentId { get; set; }

        public string UserId { get; set; }
        public int Phone { get; set; }
        public string Address { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
