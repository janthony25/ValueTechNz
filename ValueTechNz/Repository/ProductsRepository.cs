using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ValueTechNz.Data;
using ValueTechNz.Models.Dto;
using ValueTechNz.Repository.IRepository;

namespace ValueTechNz.Repository
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly DataContext _data;
        private readonly ILogger<ProductsRepository> _logger;

        public ProductsRepository(DataContext data, ILoggerFactory loggerFactory)
        {
            _data = data;
            _logger = loggerFactory.CreateLogger<ProductsRepository>();
        }
        public async Task<List<GetProductsDto>> GetAllProductsAsync()
        {
            try
            {
                // Get Products List
                var products = await _data.Products
                    .Include(p => p.ProductCategory)
                        .ThenInclude(pc => pc.Category)
                    .Select(p => new GetProductsDto
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        Brand = p.Brand,
                        Price = p.Price,
                        Description = p.Description,
                        ImageFileName = p.ImageFileName,
                        CategoryName = p.ProductCategory.Select(pc => pc.Category.CategoryName).FirstOrDefault()
                    }).ToListAsync();

                _logger.LogInformation($"Fetch successful! retrieved {products.Count} products.");
                return products;
                
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to fetch product list.");
                throw;
            }
        }
    }
}
