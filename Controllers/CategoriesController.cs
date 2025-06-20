using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Data;
using ECommerceAPI.CreateUserDtos;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;



namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CategoriesController(ApplicationDbContext _dbContext) : ControllerBase
    {
        [HttpPost("add-category")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddCategory(CategoryDto model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.Image == null || model.Image.Length == 0)
                return BadRequest("Image is required.");

            // Generate unique file name
            var fileName = model.Image.FileName;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/categories", fileName);

            // Save the image to wwwroot/images
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.Image.CopyToAsync(stream);
            }

            var category = new Category()
            {
                Name = model.Name,
                Description= model.Description,
                ImagePath = $"images/categories/{fileName}"
            };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return Ok(new {Message = "Category is Created Successfully!"});
        }

        [HttpGet("get-all-categories")]
        [Authorize(Roles = "Customer, Admin")]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
        {
            var categories = _dbContext.Categories.ToList();
            return Ok(categories);
        }

        [HttpGet("get-category-by-id/{id}")]
        [Authorize(Roles = "Customer, Admin")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _dbContext.Categories.FindAsync(id);
            if(category == null)
                return NotFound(new {Message = $"Category with id = {id} is not found"});

            

            return Ok(new {Name = category.Name, Description = category.Description});
        }

        [HttpPut("update-category/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateCategory(int id, [FromForm]CategoryDto model)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);

            var category = _dbContext.Categories.Find(id);
            if (category == null)
                return NotFound(new { Message = $"Category with id = {id} is not found" });


            if (model.Image == null || model.Image.Length == 0)
                return BadRequest("Image is required.");

            // Generate unique file name
            var fileName = model.Image.FileName;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/categories", fileName);

            // Save the image to wwwroot/images
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.Image.CopyToAsync(stream);
            }

            category.Name = model.Name;
            category.Description = model.Description;
            category.ImagePath = $"images/categories/{fileName}";
            await _dbContext.SaveChangesAsync();
          
            return Ok(new {Message = $"Category with id = {id} is updated successfully!"});

        }


        [HttpDelete("delete-category/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = _dbContext.Categories.Find(id);
            if (category == null)
                return NotFound(new { Message = $"Category with id = {id} is not found" });

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();

            return Ok(new {Message = $"Category deleted successfully!" });

        }

        

    }
}
