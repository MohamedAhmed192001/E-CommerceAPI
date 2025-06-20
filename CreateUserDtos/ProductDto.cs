namespace ECommerceAPI.CreateUserDtos
{
    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string UnitSize { get; set; }
        public string Sku { get; set; }
        public IFormFile ImageFile { get; set; }

        public int CategoryId { get; set; }
    }
}
