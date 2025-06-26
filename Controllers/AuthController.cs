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
using ECommerceAPI.EmailConfirmation;
using System.Net;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(UserManager<ApplicationUser> _userManager,
        ApplicationDbContext _dbContext, JwtOptions _jwtOptions, EmailSender _emailSender) : ControllerBase
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

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token); // Important to URL-encode the token

            var confirmationUrl = $"{Request.Scheme}://{Request.Host}/api/auth/confirm-email?userId={user.Id}&token={encodedToken}";

            await _emailSender.SendEmailAsync(user.Email, "Confirm your account",
                $"<p>Please <a href = '{confirmationUrl}'>Click here</a> to confirm your email.</p>");


            return Ok(new { message = "Register successed, Please check your email to confirm your account." });
       }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery]string userId, [FromQuery] string token)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            if (userId == null || token == null)
                return BadRequest(new { message = "Missing user ID or token" });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found!" });
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                return BadRequest();

            return Redirect("http://localhost:55688/email-confirmed");

        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn(LogInDto model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var user = await _userManager.FindByEmailAsync(model.Email);
            
            // ✅ Check if email is confirmed
            if (!await _userManager.IsEmailConfirmedAsync(user))
                return Unauthorized(new { message = "Please confirm your email before logging in." });

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized(new { message = "Invalid credentials" });

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

    }
}
