using Microsoft.EntityFrameworkCore;
using ValueTechNz.Data;
using ValueTechNz.Helpers;
using ValueTechNz.Models.Dto;
using ValueTechNz.Repository.IRepository;

namespace ValueTechNz.Repository
{
    public class StoreRepository : IStoreRepository
    {
        private readonly DataContext _data;
        private readonly ILogger<StoreRepository> _logger;
        public StoreRepository(DataContext data, ILoggerFactory logger)
        {
            _data = data;
            _logger = logger.CreateLogger<StoreRepository>();
        }
        public async Task<PaginatedList<GetProductsDto>> GetStoreProductsAsync(int pageNumber, int pageSize, string? searchTerm)
        {
            try
            {
                var query = _data.Products
                .Include(p => p.ProductCategory)
                    .ThenInclude(pc => pc.Category)
                .AsQueryable();

                // Apply search filter if search term is provided
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(p =>
                        p.ProductName.ToLower().Contains(searchTerm) ||
                        p.Brand.ToLower().Contains(searchTerm) ||
                        p.ProductCategory.Any(pc => pc.Category.CategoryName.ToLower().Contains(searchTerm))
                        );
                }

                var finalQuery = query.Select(p => new GetProductsDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Brand = p.Brand,
                    Price = p.Price,
                    CategoryName = p.ProductCategory
                            .Select(pc => pc.Category.CategoryName)
                            .FirstOrDefault(),
                    Description = p.Description,
                    ImageFileName = p.ImageFileName,
                    DateAdded = p.DateAdded
                });

                return await PaginatedList<GetProductsDto>.CreateAsync(finalQuery, pageNumber, pageSize);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to fetch product list.");
                throw;
            }


        }
    }
}
