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

            if(model == null || model.Items.Count == 0 || !model.Items.Any())
                return BadRequest(new {Message = "Invalid order data!"});

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; 

            var order = new Order()
            {
                UserId = userId,
                Phone = model.Phone,
                Address = model.Address,    
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = 0,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in model.Items) 
            {
                var product = await _dbContext.Products.FindAsync(item.ProductId);
                if (product == null)
                    return NotFound(new {Message = $"Product id = {item.ProductId} not found"});

                if (product.Stock < item.Quantity)
                    return BadRequest(new { Message = $"Not enough stock for product: {product.Name}" });

                product.Stock -= item.Quantity;

                var orderItem = new OrderItem()
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceAtPurchase = product.Price
                };

                order.TotalAmount += product.Price * item.Quantity; 

                order.OrderItems.Add(orderItem);

            }

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
            return Ok(new { Message = "Order placed successfully!", orderId = order.Id });
        }

        [HttpGet("get-all-orders")]
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

        //[HttpPut("{id}")]
        //[Authorize(Roles = "Customer")]
        //public async Task<ActionResult> UpdateOrder(int id, OrderDto model)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var productIdsFromModel = model.Items.Select(p => p.ProductId).ToList();
        //    var products = _dbContext.Products.Where(p => productIdsFromModel.Contains(p.Id)).ToList();
        //    if (productIdsFromModel.Count != products.Count)
        //        return NotFound(new {Message = "One or more Products are invalid!"});

        //    var order = _dbContext.Orders.Find(id);
        //    if (order == null)
        //        return NotFound(new { Message = $"Order with id = {id} is not found!" });

        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (userId == null || userId != order.UserId)
        //        return Unauthorized();

        //    order.OrderDate = DateTime.Now;
        //    order.TotalAmount = model.TotalAmount;
        //    order.OrderItems = model.Items;

        //    await _dbContext.SaveChangesAsync();
        //    return Ok(new {Message = $"Order with id = {id} is updated successfully!"});

        //}

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
