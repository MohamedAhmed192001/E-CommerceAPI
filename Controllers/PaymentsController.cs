using ECommerceAPI.CreateUserDtos;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Security.Claims;
namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController(ApplicationDbContext _dbContext) : ControllerBase
    {
        [HttpPost("place-and-pay")]
        public async Task<IActionResult> PlaceOrderAndCreateCheckoutSession(OrderDto orderDto)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (orderDto == null || orderDto.Items.Count == 0 || !orderDto.Items.Any())
                return BadRequest(new { Message = "Invalid order data!" });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var order = new Order()
            {
                UserId = userId,
                Phone = orderDto.Phone,
                Address = orderDto.Address,
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = 0,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in orderDto.Items)
            {
                var product = await _dbContext.Products.FindAsync(item.ProductId);
                if (product == null)
                    return NotFound(new { Message = $"Product id = {item.ProductId} not found" });

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
            

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = order.OrderItems.Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)item.PriceAtPurchase * 1000,
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name
                        }
                    },
                    Quantity = (long)item.Quantity
                }).ToList(),
                Mode = "payment",
                SuccessUrl = $"https://localhost:55688/payment/payment-success/{order.Id}",
                CancelUrl = "https://localhost:55688/cart"
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Ok(new { SessionId = session.Id });
        }
    }

}
