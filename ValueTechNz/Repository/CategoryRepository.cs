using Microsoft.EntityFrameworkCore;
using ValueTechNz.Data;
using ValueTechNz.Models.Dto;
using ValueTechNz.Repository.IRepository;

namespace ValueTechNz.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _data;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(DataContext data, ILoggerFactory loggerFactory)
        {
            _data = data;
            _logger = loggerFactory.CreateLogger<CategoryRepository>();
        }

        public async Task<List<CategoryListDto>> GetCategoryListAsync()
        {
            try
            {
                // Fetch category list
                var categories = await _data.Categories
                    .Select(c => new CategoryListDto {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName
                    }).ToListAsync();

                _logger.LogInformation($"Fetch success, returning {categories.Count} categories");
                return categories;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unable to fetche categories");
                throw;
            }
        }
    }
}
