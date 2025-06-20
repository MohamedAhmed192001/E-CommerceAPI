namespace ECommerceAPI.CreateUserDtos
{
    public class CategoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
    }
}
