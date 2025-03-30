namespace ECommerceAPI.ResponseDtos
{
    public class ReviewResponseDto
    {
        public string UserName { get; set; }
        public string ProductName { get; set; }
        public string Comment { get; set; }
        public int Rate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
