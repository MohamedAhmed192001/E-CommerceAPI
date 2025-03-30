using ECommerceAPI.Models;

namespace ECommerceAPI.CreateUserDtos
{
    public class UserDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
