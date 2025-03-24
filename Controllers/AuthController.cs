using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(UserManager<ApplicationUser> _userManager,
        ApplicationDbContext _dbContext, JwtOptions _jwtOptions) : ControllerBase
    {
       [HttpPost("Register")]
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

        [HttpPost("LogIn")]
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
                     new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                     new Claim(ClaimTypes.Email, user.Email),
                     new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),

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
    }
}
