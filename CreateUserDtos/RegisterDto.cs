using ECommerceAPI.Models;

namespace ECommerceAPI.CreateUserDtos
{
    public class RegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }  // Admin, Customer
        public string Password { get; set; }
    }
}
