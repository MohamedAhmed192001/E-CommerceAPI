using ECommerceAPI.CreateUserDtos;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.ResponseDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ReviewsController(ApplicationDbContext _dbContext) : ControllerBase
    {
        [HttpPost("WriteReview")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> WriteReview(ReviewDto model)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return Unauthorized();

            var product = await _dbContext.Products.FindAsync(model.ProductId);
            if (product == null)
                return NotFound(new {Message = "Product is not found!"});

            var review = new Review
            {
                UserId = userId,
                ProductId = model.ProductId,
                Comment = model.Comment,
                Rate = model.Rate,
                CreatedAt = DateTime.UtcNow,
            };

            await _dbContext.Reviews.AddAsync(review);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Review submitted successfully!" });

        }

        [HttpGet("GetAllReviews")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Review>>> GetAllReviews()
        {
            var reviews = _dbContext.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .ToList();

            var result = reviews.Select(r => new ReviewResponseDto
            {
                UserName = r.User.UserName!,
                ProductName = r.Product.Name,   
                Comment = r.Comment,
                Rate = r.Rate,
                CreatedAt = DateTime.UtcNow,
            }).ToList();

            return Ok(result);
        }

        [HttpGet("GetUserReviews")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<Review>> GetUserReviews()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var userReviews = await _dbContext.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .Where(r => r.UserId == userId)
                .ToListAsync();
            if (!userReviews.Any())
                return NotFound(new { Message = "User has no any review!" });

            var result = userReviews.Select(r => new ReviewResponseDto
            {
                UserName = r.User.UserName!,
                ProductName = r.Product.Name,
                Comment = r.Comment,
                Rate = r.Rate,
                CreatedAt = DateTime.UtcNow,
            }).ToList();

            return Ok(result);
        }

        [HttpPut("UpdateReview/{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> UpdateReview(int id, ReviewDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return Unauthorized();

            var product = _dbContext.Products.Find(model.ProductId);
            if (product == null)
                return NotFound(new { Message = "Product is not found!" });

            var review = _dbContext.Reviews.Where(r => r.UserId == userId && r.Id == id).FirstOrDefault();
            if (review == null)
                return NotFound(new { Message = $"No review with id = {id}" });

            review.ProductId = model.ProductId;
            review.Comment = model.Comment;
            review.Rate = model.Rate;
            review.CreatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return Ok(new { Message = "Review updated successfully!" });
        }

        [HttpDelete("DeleteReview/{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> DeleteReview(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return Unauthorized();

            var review = _dbContext.Reviews.Where(r => r.UserId == userId && r.Id == id).FirstOrDefault();
            if (review == null)
                return NotFound(new {Message = $"No review with id = {id}" });


            _dbContext.Reviews.Remove(review);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Review is deleted successfully!" });
        }

    }
}
