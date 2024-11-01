using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
using ValueTechNz.Data;
using ValueTechNz.Models;
using ValueTechNz.Models.Dto;
using ValueTechNz.Repository.IRepository;

namespace ValueTechNz.Repository
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly DataContext _data;
        private readonly ILogger<ProductsRepository> _logger;
        private readonly IWebHostEnvironment _environment;

        public ProductsRepository(DataContext data, ILoggerFactory loggerFactory, IWebHostEnvironment environment)
        {
            _data = data;
            _environment = environment;
            _logger = loggerFactory.CreateLogger<ProductsRepository>();
        }

        public async Task AddProductAsync(AddUpdateProductDto addProductDto)
        {
            try
            {
                string newFileName = null;

                if(addProductDto.ImageFile != null && addProductDto.ImageFile.Length > 0)
                {
                    newFileName = DateTime.Now.ToString("yyyyMMddhhssfff");
                    newFileName += Path.GetExtension(addProductDto.ImageFile!.FileName);

                    string imageFullPath = _environment.WebRootPath + "/img/" + newFileName;
                    using (var stream = System.IO.File.Create(imageFullPath))
                    {
                        addProductDto.ImageFile.CopyTo(stream);
                    }
                }
                else
                {
                    // Assign default image
                    newFileName = "No image.png";
                }

                // Add new product
                var product = new Product
                {
                    ProductName = addProductDto.ProductName,
                    Brand = addProductDto.Brand,
                    Price = addProductDto.Price,
                    Description = addProductDto.Description,
                    ImageFileName = newFileName
                };

                // Add new product to DB
                _data.Products.Add(product);
                await _data.SaveChangesAsync();

                // Create new ProductCategory entity
                var productCategory = new ProductCategory
                {
                    ProductId = product.ProductId,
                    CategoryId = addProductDto.CategoryId
                };

                // Save product category to DB
                _data.ProductCategories.Add(productCategory);
                await _data.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding product");
                throw;
            }
        }

        public async Task<List<GetProductsDto>> GetAllProductsAsync()
        {
            try
            {
                // Get Products List
                var products = await _data.Products
                    .Include(p => p.ProductCategory)
                        .ThenInclude(pc => pc.Category)
                    .OrderByDescending(p => p.DateAdded)
                    .Select(p => new GetProductsDto
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        Brand = p.Brand,
                        Price = p.Price,
                        CategoryName = p.ProductCategory.Select(pc => pc.Category.CategoryName).FirstOrDefault(),
                        Description = p.Description,
                        ImageFileName = p.ImageFileName,
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

        public async Task<AddUpdateProductDto> GetProductByIdAsync(int id)
        {
            try
            {
                // Fetch product by id
                var product = await _data.Products
                    .Include(p => p.ProductCategory)
                        .ThenInclude(pc => pc.Category)
                    .Where(pc => pc.ProductId == id)
                    .Select(p => new AddUpdateProductDto
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        Brand = p.Brand,
                        CategoryId = p.ProductCategory.Select(pc => pc.Category.CategoryId).FirstOrDefault(),
                        Price = p.Price,
                        Description = p.Description,
                        DateUpdated = DateTime.Now
                    }).FirstOrDefaultAsync();

                if (product == null || product.ProductId == 0)
                {
                    _logger.LogWarning($"Product with id {id} not found.");
                    throw new KeyNotFoundException("Product not found.");
                }

                _logger.LogInformation($"Fetch successful. Returning product with id {product.ProductId}.");
                return product;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching product details with id {id}");
                throw;
            }
        }
    }
}
