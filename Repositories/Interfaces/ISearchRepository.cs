using ECommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Repositories.Interfaces
{
    public interface ISearchRepository
    {
        Task<Product> SearchProductByIdAsync(int  productId);
    }
}
