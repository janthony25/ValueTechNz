using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.CodeDom;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
using ValueTechNz.Data;
using ValueTechNz.Helpers;
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

        public async Task DeleteProductAsync(int id)
        {
            try
            {
                // Find product by ID
                var product = await _data.Products.FindAsync(id);

                if(product == null || product.ProductId == 0)
                {
                    _logger.LogError($"Unable to find product with id {id}");
                    throw new KeyNotFoundException($"Unable to find product");
                }

                string imageFullPath = _environment.WebRootPath + "/img/" + product.ImageFileName;
                System.IO.File.Delete(imageFullPath);

                // Delete product 
                _data.Products.Remove(product);
                await _data.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the product.");
                throw;
            }
        }

        public async Task<List<GetProductsDto>> GetLatestProductsAsync()
        {
            try
            {
                // Take 4 latest products
                var latestProducts = await _data.Products
                        .Include(p => p.ProductCategory)
                            .ThenInclude(pc => pc.Category)
                        .OrderByDescending(p => p.DateAdded)
                        .Take(4)
                        .Select(p => new GetProductsDto
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            Brand = p.Brand,
                            CategoryName = p.ProductCategory.Select(pc => pc.Category.CategoryName)
                                        .FirstOrDefault(),
                            Price = p.Price,
                            ImageFileName = p.ImageFileName
                        }).ToListAsync();

                _logger.LogInformation($"Latest product retrieved successfully!");
                return latestProducts;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving latest products.");
                throw;
            }
        }

        public async Task<PaginatedList<GetProductsDto>> GetPaginatedProductsAsync(int pageNumber, int pageSize, string? searchTerm, string sortColumn = "DateAdded", string sortOrder = "desc")
        {
            try
            {
                var query = _data.Products
                       .Include(p => p.ProductCategory)
                           .ThenInclude(pc => pc.Category)
                       .AsQueryable();

                // Apply search filter if searc term is provided
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(p =>
                        p.ProductName.ToLower().Contains(searchTerm) ||
                        p.Brand.ToLower().Contains(searchTerm) ||
                        p.ProductCategory.Any(pc => pc.Category.CategoryName.ToLower().Contains(searchTerm))
                        );
                }

                // Apply sorting based on column
                query = sortColumn.ToLower() switch
                {
                    "id" => sortOrder == "desc" ? query.OrderByDescending(p => p.ProductId) : query.OrderBy(p => p.ProductId),
                    "name" => sortOrder == "desc" ? query.OrderByDescending(p => p.ProductName) : query.OrderBy(p => p.ProductName),
                    "brand" => sortOrder == "desc" ? query.OrderByDescending(p => p.Brand) : query.OrderBy(p => p.Brand),
                    "category" => sortOrder == "desc" ?
                        query.OrderByDescending(p => p.ProductCategory.FirstOrDefault().Category.CategoryName) :
                        query.OrderBy(p => p.ProductCategory.FirstOrDefault().Category.CategoryName),
                    "price" => sortOrder == "desc" ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                    "dateadded" => sortOrder == "desc" ? query.OrderByDescending(p => p.DateAdded) : query.OrderBy(p => p.DateAdded),
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

        public async Task<GetProductsDto> GetProductDetailsAsync(int id)
        {
            try
            {
                // Find product by id
                var product = await _data.Products
                        .Include(p => p.ProductCategory)
                            .ThenInclude(pc => pc.Category)
                        .Where(p => p.ProductId == id)
                        .Select(p => new GetProductsDto
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            Brand = p.Brand,
                            Price = p.Price,
                            CategoryName = p.ProductCategory.Select(pc => pc.Category.CategoryName).FirstOrDefault(),
                            Description = p.Description,
                            ImageFileName = p.ImageFileName,
                            DateAdded = p.DateAdded,
                            DateUpdated = p.DateUpdated
                        }).FirstOrDefaultAsync();

                if(product == null || product.ProductId == 0)
                {
                    _logger.LogError($"Product with id {product.ProductId} not found.");
                    throw new KeyNotFoundException("Product not found.");
                }

                _logger.LogInformation($"Product with id {product.ProductId} fetched successfully.");
                return product;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching product details.");
                throw;
            }
        }

        public async Task UpdateProductAsync(int id, AddUpdateProductDto updateProductDto)
        {
            try
            {
                // Find product by ID
                var product = await _data.Products.FindAsync(id);

                if (product == null || product.ProductId == 0)
                {
                    _logger.LogError($"Product with id {id} not found.");
                    throw new KeyNotFoundException("Product not found.");
                }

                // Update the image file if we want a new one
                string newFileName = product.ImageFileName;
                if(updateProductDto.ImageFile != null && updateProductDto.ImageFile.Length > 1)
                {
                    newFileName = DateTime.Now.ToString("yyyyMMddHHssfff");
                    newFileName += Path.GetExtension(updateProductDto.ImageFile.FileName);

                    string imageFullPath = _environment.WebRootPath + "/img/" + newFileName;
                    using (var stream = System.IO.File.Create(imageFullPath))
                    {
                        updateProductDto.ImageFile.CopyTo(stream);
                    }

                    // delete the old image
                    string oldImageFullPath = _environment.WebRootPath + "/img/" + product.ImageFileName;
                    System.IO.File.Delete(oldImageFullPath);
                }


                // Update product details
                product.ProductName = updateProductDto.ProductName;
                product.Brand = updateProductDto.Brand;
                product.Price = updateProductDto.Price;
                product.Description = updateProductDto.Description;
                product.ImageFileName = newFileName;
                product.DateUpdated = DateTime.Now;

                // Find Product Category relationship
                var existingProductCategory = await _data.ProductCategories
                        .FirstOrDefaultAsync(pc => pc.ProductId == product.ProductId);
                
                // Update <Product Category> if the category has been changed
                if (existingProductCategory.CategoryId != updateProductDto.CategoryId)
                {
                    // Remove existing relationship
                    _data.ProductCategories.Remove(existingProductCategory);

                    // Create new relationship
                    var newProductCategory = new ProductCategory
                    {
                        ProductId = product.ProductId,
                        CategoryId = updateProductDto.CategoryId
                    };

                    await _data.ProductCategories.AddAsync(newProductCategory);
                }

                await _data.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updatind product with id {id}");
                throw;
            }
        }
    }
}
