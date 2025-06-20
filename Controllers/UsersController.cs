using ECommerceAPI.CreateUserDtos;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.ResponseDtos;
using ECommerceAPI.UpdateDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController(ApplicationDbContext _dbContext, 
        UserManager<ApplicationUser> _userManager) : ControllerBase
    {

        [HttpGet("get-all-users")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllUsers()
        {
            var users = _dbContext.Users.Select(user => new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.UserName,
                user.Email,
                user.Address,
                user.Role,
            });
            return Ok(users);
        }

        [HttpGet("get-user/{userId}")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetUser(string userId)
        {
            var user = await _dbContext.Users.Select(user => new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.UserName,
                user.Email,
                user.Address,
                user.Role,

            }).FirstOrDefaultAsync(u => u.Id == userId);

            return Ok(user);
        }

        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser(RegisterDto model)
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

            return Ok(new { Message = "User added Successfully!" });
        }

        [HttpPut("update-user/{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, UpdateUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _userManager.FindByIdAsync(userId).Result;
            if (user == null)
                return NotFound(new { Message = "User not found!" });


            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.FirstName + model.LastName;
            user.Email = model.Email;
            user.Address = model.Address;
            user.Role = model.Role;
           
           await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "User updated Successfully!" });
        }

        [HttpPost("AssignRoleToUser")]
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

        [HttpDelete("delete-user/{userId}")]
        public async Task<ActionResult> DeleteUser(string  userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { Message = "User not found!" });

            await _userManager.DeleteAsync(user);
            await _dbContext.SaveChangesAsync();

            return Ok(new {Message = "User deleted successfully!"});
        }



    }
}
