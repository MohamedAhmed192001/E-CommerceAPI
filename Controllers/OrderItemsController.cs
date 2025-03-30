using ECommerceAPI.Data;
using ECommerceAPI.CreateUserDtos;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class OrderItemsController(ApplicationDbContext _dbContext) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> CreateOrderItem(OrderItemDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingOrder = await _dbContext.Orders.AnyAsync(o => o.Id == model.OrderId);
            if (!existingOrder)
                return NotFound(new { Message = $"Order with id = {model.OrderId} is not found!" });

            var existingProduct = await _dbContext.Products.AnyAsync(p => p.Id == model.ProductId);
            if (!existingProduct)
                return NotFound(new { Message = $"Product with id = {model.ProductId} is not found!" });

            var orderItem = new OrderItem()
            {
                OrderId = model.OrderId,
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                Price = model.Price
            };

            await _dbContext.OrderItems.AddAsync(orderItem);
            await _dbContext.SaveChangesAsync();
            
            return Ok(new {Message = $"OrderItem is created successfully!" });

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetAllOrderItems()
        {
            var orderItems = _dbContext.OrderItems.ToList();
            return Ok(orderItems);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItem>> GetOrderItem(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var orderItem = await _dbContext.OrderItems.FindAsync(id);
            if (orderItem == null)
                return NotFound(new {Message = $"OrderItem with id = {id} is not found!"});

            return Ok(orderItem);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateOrderItem(int id, OrderItemDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var orderItem = await _dbContext.OrderItems.FindAsync(id);
            if (orderItem == null)
                return NotFound(new { Message = $"OrderItem with id = {id} is not found!" });

            var existingOrder = await _dbContext.Orders.AnyAsync(o => o.Id == model.OrderId);
            if (!existingOrder)
                return NotFound(new { Message = $"Order with id = {model.OrderId} is not found!" });

            var existingProduct = await _dbContext.Products.AnyAsync(p => p.Id == model.ProductId);
            if (!existingProduct)
                return NotFound(new { Message = $"Product with id = {model.ProductId} is not found!" });

            orderItem.OrderId = model.OrderId;
            orderItem.ProductId = model.ProductId;
            orderItem.Quantity = model.Quantity;
            orderItem.Price = model.Price;

            await _dbContext.SaveChangesAsync();

            return Ok(new {Message = $"OrderItem with id = {id} is updated successfully!"});
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrderItem(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var orderItem = await _dbContext.OrderItems.FindAsync(id);
            if (orderItem == null)
                return NotFound(new { Message = $"OrderItem with id = {id} is not found!" });

            _dbContext.OrderItems.Remove(orderItem);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = $"OrderItem with id = {id} is deleted successfully!" });
        }
    }
}
