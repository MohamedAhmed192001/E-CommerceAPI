using ECommerceAPI.CreateUserDtos;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController(ICartRepository _cartRepository) : ControllerBase
    {
        [HttpPost("add-to-cart")]
        [Authorize()]
        public async Task<IActionResult> AddToCart(CartItemDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user == null)
                return Unauthorized();

            await _cartRepository.AddToCartAsync(user, model.ProductId, model.Quantity, model.Price);
            return Ok(new { Message = "Item added to cart!" });
        }

        [HttpGet("get-cart-by-userId")]
        [Authorize]
        public async Task<ActionResult<Cart>> GetCartByUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _cartRepository.GetCartByUserIdAsync(userId);
        }

        [HttpDelete("remove-from-cart")]
        [Route("{productId}")]
        [Authorize]
        public async Task<ActionResult> RemoveFromCart(int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _cartRepository.RemoveFromCartAsync(userId, productId);
            return Ok(new { Message = "Item removed successfully!" });
        }

        [HttpDelete("clear-cart")]
        public async Task<ActionResult> ClearCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _cartRepository.ClearCartAsync(userId);
            return Ok(new { Message = "Cart cleared successfully!" });
        }


    }
}
