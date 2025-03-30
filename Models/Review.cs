namespace ECommerceAPI.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public string Comment { get; set; }
        public int Rate { get; set; }
        public DateTime CreatedAt { get; set; }

        public ApplicationUser User { get; set; }
        public Product Product { get; set; }
    }
}
