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
        public async Task<PaginatedList<GetProductsDto>> GetStoreProductsAsync(int pageNumber,
                                                                              int pageSize,
                                                                              string? searchTerm,
                                                                              string? brand,
                                                                              string? category,
                                                                              string? sort)
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

                // Apply brand filter
                if (!string.IsNullOrWhiteSpace(brand))
                {
                    query = query.Where(p => p.Brand.ToLower() == brand.ToLower());
                }

                // Apply category filter
                if (!string.IsNullOrWhiteSpace(category))
                {
                    query = query.Where(p => p.ProductCategory.Any(pc => pc.Category.CategoryName.ToLower() == category.ToLower()));
                }

                // Apply sorting by date
                query = sort?.ToLower() switch
                {
                    "price_asc" => query.OrderBy(p => p.Price),
                    "price_desc" => query.OrderByDescending(p => p.Price),
                    "newest" => query.OrderByDescending(p => p.DateAdded),
                    _ => query.OrderByDescending(p => p.DateAdded)
                };

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
