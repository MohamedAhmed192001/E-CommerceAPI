using ECommerceAPI.Models;

namespace ECommerceAPI.ResponseDtos
{
    public class UserResponseDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public IEnumerable<OrderResponseDto> Orders { get; set; } = new List<OrderResponseDto>();
    }
}
