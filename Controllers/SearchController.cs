using ECommerceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Repositories;
using ECommerceAPI.Repositories.Interfaces;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController(ISearchRepository _searchRepository) : ControllerBase
    {
        [HttpGet("search-product-by-id/{id}")]
        
        public async Task<IActionResult> SearchProductById(int id)
        {
            var product = await _searchRepository.SearchProductByIdAsync(id);
            if (product == null) 
                return NotFound(new {Message = $"No product found with id = {id}"});

            return Ok(product);
        }
    }
}
