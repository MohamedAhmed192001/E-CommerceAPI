using ECommerceAPI.Data;
using ECommerceAPI.CreateUserDtos;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ECommerceAPI.ResponseDtos;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(ApplicationDbContext _dbContext) : ControllerBase
    {
        [HttpPost("place-order")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> PlaceOrder(OrderDto model)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var productIdsFromModel = model.OrderItems.Select(x => x.ProductId).ToList();
            var products = _dbContext.Products.Where(p => productIdsFromModel.Contains(p.Id)).ToList();
            if(products.Count != productIdsFromModel.Count)
                return NotFound(new {Message = "One or more products are invalid "});

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var order = new Order()
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = model.TotalAmount,
                OrderItems = model.OrderItems.Select(oi => new OrderItem
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                }).ToList(),
            };


            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();
            return Ok(new { Message = "Order is created successfully!" });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            var orders = await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();

            if (orders == null || orders.Count == 0) 
                return NotFound(new {Message = "No orders found!"});

            var result = orders.Select(o => new OrderResponseDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                UserEmail = o.User.Email,
                OrderItems = o.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    ProductName = oi.Product.Name,
                    Price = oi.Price,
                    Quantity = oi.Quantity
                }).ToList()

            }).ToList();


            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = _dbContext.Orders.Find(id);
            if (order == null)
                return NotFound(new {Message = $"Order with id = {id} is not found!" });

            return Ok(order);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> UpdateOrder(int id, OrderDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productIdsFromModel = model.OrderItems.Select(p => p.ProductId).ToList();
            var products = _dbContext.Products.Where(p => productIdsFromModel.Contains(p.Id)).ToList();
            if (productIdsFromModel.Count != products.Count)
                return NotFound(new {Message = "One or more Products are invalid!"});

            var order = _dbContext.Orders.Find(id);
            if (order == null)
                return NotFound(new { Message = $"Order with id = {id} is not found!" });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null || userId != order.UserId)
                return Unauthorized();

            order.OrderDate = DateTime.Now;
            order.TotalAmount = model.TotalAmount;
            order.OrderItems = model.OrderItems;

            await _dbContext.SaveChangesAsync();
            return Ok(new {Message = $"Order with id = {id} is updated successfully!"});

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var order = _dbContext.Orders.Find(id);
            if (order == null)
                return NotFound(new { Message = $"Order with id = {id} is not found!" });
            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();
            return Ok(new { Message = $"Order with id = {id} is deleted successfully!" });
        }

    }
}
