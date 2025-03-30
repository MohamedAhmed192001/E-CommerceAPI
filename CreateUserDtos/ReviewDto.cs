namespace ECommerceAPI.CreateUserDtos
{
    public class ReviewDto
    {
        public int ProductId { get; set; }
        public string Comment { get; set; }
        public int Rate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
