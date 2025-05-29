using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.CreateUserDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ECommerceAPI.ResponseDtos;
using ECommerceAPI.Repositories.Interfaces;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(UserManager<ApplicationUser> _userManager,
        ApplicationDbContext _dbContext, JwtOptions _jwtOptions) : ControllerBase
    {
       [HttpPost("register")]
       public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var user = new ApplicationUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.FirstName + model.LastName,
                Email = model.Email,
                Address = model.Address,
                Role = model.Role,
                PasswordHash = model.Password,
            };


            var result = await _userManager.CreateAsync(user, user.PasswordHash);
            if (!result.Succeeded)
                return BadRequest(result);
            
            return Ok(new { Message = "User Registered Successfully!" });
       }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn(LogInDto model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized(new {Message = "Invalid credentials" });

            var token = GenerateJwtTokent(user);

            return Ok(new { Token = token });
        }

        private string GenerateJwtTokent(ApplicationUser user)
        {
            var key = Encoding.UTF8.GetBytes(_jwtOptions.SecurityKey);
            var securityKey = new SymmetricSecurityKey(key);
            var Credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new Claim[]
                 {
                     new Claim(ClaimTypes.NameIdentifier, user.Id),
                     new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                     new Claim(ClaimTypes.Email, user.Email),
                     new Claim(ClaimTypes.Role, user.Role)

                 };

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                signingCredentials: Credentials,
                expires: DateTime.UtcNow.AddMinutes(60),
                claims: claims
                );

            var jwtTokenHandler = new JwtSecurityTokenHandler();

            return jwtTokenHandler.WriteToken(jwtSecurityToken);
        }

        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllUsers()
        {
            var users = _dbContext.Users.ToList();
            return Ok(users);
        }

        [HttpPost("AssignRoleToUser")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AssignRoleToUser(AssignRoleToUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _dbContext.Users.FindAsync(model.UserId);
            if (user == null)
                return NotFound(new { Message = $"User with id = {model.UserId} is not found!" });

            var role = await _dbContext.Roles.FindAsync(model.RoleId);
            if (role == null)
                return NotFound(new { Message = $"Role with id = {model.RoleId} is not found!" });

            await _userManager.AddToRoleAsync(user, role.Name);
            await _dbContext.SaveChangesAsync();


            return Ok(new { Message = $"{role.Name} Role is assigned to UserName = {user.UserName}" });

        }

        [HttpGet("GetUserDetails")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<ApplicationUser>> GetUserDetails()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _dbContext.Users
                .Include(u => u.Orders)
                .ThenInclude(o => o.OrderItems)
                .Select(u => 
            new UserResponseDto
            { 
                Id = u.Id,
                FullName = u.FirstName + " " + u.LastName,
                Email = u.Email,
                Orders = u.Orders.Select(o => 
                new OrderResponseDto
                {
                    UserId = o.UserId,
                    UserName = o.User.UserName,
                    Address = o.User.Address,
                    OrderDate = o.OrderDate,
                    Phone = o.Phone,
                    Items = o.OrderItems.Select(oi => new OrderItemResponseDto
                    {
                        ProductName = oi.Product.Name,
                        Price = oi.PriceAtPurchase,
                        Quantity = oi.Quantity
                    }).ToList()
                }).ToList() 

            }).FirstOrDefaultAsync(u => u.Id == userId);

            return Ok(user);

        }
    }
}
