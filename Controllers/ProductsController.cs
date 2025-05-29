using ECommerceAPI.Data;
using ECommerceAPI.CreateUserDtos;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(ApplicationDbContext _dbContext) : ControllerBase
    {
        [HttpPost("add-product")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddProduct(ProductDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingCategory = await _dbContext.Categories.AnyAsync(c => c.Id == model.CategoryId);
            if (!existingCategory)
                return NotFound(new { Message = $"Category with id = {model.CategoryId} is not found!" });



            if (model.ImageFile == null || model.ImageFile.Length == 0)
                return BadRequest("Image is required.");

            // Generate unique file name
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            // Save the image to wwwroot/images
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.ImageFile.CopyToAsync(stream);
            }

            // Save to DB
            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                CategoryId = model.CategoryId,
                ImagePath = "/images/" + fileName // Save only relative path
            };

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Product is created successfully!" });
        }

        [HttpGet]
        [Authorize(Roles = "Customer, Admin")]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            var products = _dbContext.Products.Include(p => p.Category).ToList();
            return Ok(products);
        }

        [HttpGet("get-product-details/{id}")]
        [Authorize(Roles = "Customer, Admin")]
        public async Task<ActionResult<Product>> GetProductDetails(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _dbContext.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound(new { Message = $"Product with id = {id} is not found!" });

            return Ok(product);
        }

        [HttpPut("update-product/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateProduct(int id, [FromForm]ProductDto model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
                return NotFound(new { Message = $"Product with id = {id} is not found!" });

            var existingCategory = await _dbContext.Categories.AnyAsync(c => c.Id == model.CategoryId);
            if (!existingCategory)
                return NotFound(new { Message = $"Category with id = {model.CategoryId} is not found!" });

            product.Name = model.Name;
            product.Price = model.Price;
            product.Stock = model.Stock;
            product.Description = model.Description;
            product.CategoryId = model.CategoryId;

            await _dbContext.SaveChangesAsync();
            
            return Ok(new {Message = $"Product with id = {id} is updated successfully!"});
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
                return NotFound(new { Message = $"Product with id = {id} is not found!" });

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();

            return Ok(new {Message =  $"Product with id = {id} is deleted successfully!" });
        }
    }
}
