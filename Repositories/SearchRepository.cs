using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Repositories
{
    public class SearchRepository(ApplicationDbContext _dbContext) : ISearchRepository
    {
        public async Task<Product> SearchProductByIdAsync(int productId)
        {
            return await _dbContext.Products.FindAsync(productId);
        }
    }
}
