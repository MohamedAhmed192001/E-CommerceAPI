using ECommerceAPI.Data;
using ECommerceAPI.CreateUserDtos;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CategoriesController(ApplicationDbContext _dbContext) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateCategory(CategoryDto model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = new Category()
            {
                Name = model.Name
            };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return Ok(new {Message = "Category is Created Successfully!"});
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
        {
            var categories = _dbContext.Categories.ToList();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _dbContext.Categories.FindAsync(id);
            if(category == null)
                return NotFound(new {Message = $"Category with id = {id} is not found"});
            return Ok(category);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateCategory(int id, CategoryDto model)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);

            var category = _dbContext.Categories.Find(id);
            if (category == null)
                return NotFound(new { Message = $"Category with id = {id} is not found" });

            category.Name = model.Name;
            await _dbContext.SaveChangesAsync();
          
            return Ok(new {Message = $"Category with id = {id} is updated successfully!"});

        }


        [HttpDelete("{id}")]
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

            return Ok(new {Message = $"Category with id = {id} is deleted successfully!" });

        }

        

    }
}
