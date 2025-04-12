using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Repositories
{
    public class CartRepository(ApplicationDbContext _dbContext) : ICartRepository
    {
        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            var cart = await _dbContext.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            return cart;
        }

        public async Task AddToCartAsync(string userId, int productId, int quantity, decimal price)
        {
            var cart = await GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                var newcart = new Cart()
                {
                    UserId = userId,
                };
                await _dbContext.Carts.AddAsync(newcart);
            }

            var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Price = price
                });
            }

            await _dbContext.SaveChangesAsync();

        }

        public async Task RemoveFromCartAsync(string userId, int productId)
        {
            var cart = await GetCartByUserIdAsync(userId);

            var cartItem = cart.CartItems.FirstOrDefault(c => c.ProductId == productId);
            if (cartItem != null)
            {
                cart.CartItems.Remove(cartItem);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task ClearCartAsync(string userId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            if(cart != null)
            {
                cart.CartItems.Clear();
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}
