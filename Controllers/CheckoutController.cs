using ECommerceAPI.Data;
using ECommerceAPI.PaymentDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Configuration;
using System.Security.Claims;

namespace ECommerceAPI.Controllers
{
    [Authorize(Roles = "Customer")]
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController(ApplicationDbContext _dbContext, IConfiguration _config) : ControllerBase
    {
        // Create PaymentIntent

        [HttpPost("create-payment-intent")]
        public async Task<IActionResult> CreatePaymentIntent(CheckoutDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId == null ) 
                return Unauthorized();
            var order = await _dbContext.Orders.Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.UserId == userId && o.Id == model.OrderId);
            if (order == null )
                return NotFound(new {Message = "Order not found!"});

            var totalAmount = order.OrderItems.Sum(oi => (oi.Quantity * oi.PriceAtPurchase));

            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = (long)totalAmount,
                Currency = model.Currency,
                PaymentMethodTypes = new List<string> { "card" }
            });

            order.PaymentIntentId = paymentIntent.Id;
            await _dbContext.SaveChangesAsync();
            return Ok(new {clientSecret = paymentIntent.ClientSecret});
        }

        // Confirm Payment

        [HttpPost("confirm-payment")]
        public async Task<IActionResult> ConfirmPayment(PaymentConfirmationDto model)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.PaymentIntentId == model.PaymentIntentId);
            if (order == null )
                return NotFound(new {Message = "Order not found!"});

            var paymentIntentService = new PaymentIntentService();

            var paymentIntent = await paymentIntentService.GetAsync(model.PaymentIntentId);

            if(paymentIntent.Status == "succeeded")
            {
                order.Status = "Paid";
                await _dbContext.SaveChangesAsync();
                return Ok(new {Message = "Payment successful and order confirmed!"});
            }

            return BadRequest(new { Message = "Payment not successful." });

        }
    }
}
