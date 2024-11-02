
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValueTechNz.Helpers;
using ValueTechNz.Models.Dto;
using ValueTechNz.Models.ViewModels;
using ValueTechNz.Repository.IRepository;

namespace ValueTechNz.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductsController> _logger;
        private const int pageSize = 10;

        public ProductsController(IUnitOfWork unitOfWork, ILogger<ProductsController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET : Index for Product List
        public async Task<IActionResult> Products(int pageNumber = 1,
                                                  string search = null,
                                                  string sortColumn = "dateadded",
                                                  string sortOrder = "desc")
        {
            try
            {
                ViewData["CurrentSearch"] = search;
                ViewData["CurrentSort"] = sortColumn;
                ViewData["CurrentSortOrder"] = sortOrder;
                var products = await _unitOfWork.Products.GetPaginatedProductsAsync(pageNumber,
                                                                                    pageSize,
                                                                                    search,
                                                                                    sortColumn,
                                                                                    sortOrder);
                return View(products); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the product list.");
                TempData["ErrorMessage"] = "An error occurred while retrieving product list.";
                return View(new PaginatedList<GetProductsDto>(new List<GetProductsDto>(), 0, pageNumber, pageSize));
            }
        }

      

        // GET : Add Products page
        public async Task<IActionResult> AddProduct()
        {
            try
            {
                ViewBag.CategoryList = await _unitOfWork.Category.GetCategoryListAsync();
                return View();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading the Add Product page.");
                TempData["ErrorMessage"] = "An error occurred while loading the Add Product page.";
                return RedirectToAction("Products");
            }
            
        }

        // POST : Add New Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(AddUpdateProductDto addProductDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.CategoryList = await _unitOfWork.Category.GetCategoryListAsync();
                    return View(addProductDto);
                }
                await _unitOfWork.Products.AddProductAsync(addProductDto);
                TempData["SuccessMessage"] = "Product added successfully.";
                return RedirectToAction("Products");

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unable to add product.");
                TempData["ErrorMessage"] = "An error occurred while adding product.";
                return View();
            }
        }

       
        public async Task<IActionResult> UpdateProduct(int id)
        {
            try
            {
                _logger.LogInformation($"Request to retrieve details of product with id {id}");
                ViewBag.CategoryList = await _unitOfWork.Category.GetCategoryListAsync();
                var product = await _unitOfWork.Products.GetProductByIdAsync(id);
                return View(product);
            }
            catch(KeyNotFoundException)
            {
                _logger.LogError($"Product with id {id} not found.");
                TempData["KeyNotFound"] = "Product not found.";
                return View("Products");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching product details.");
                TempData["ErrorMessage"] = "An error occurred while fetching product details.";
                return View("Products");
            }
        }

        // POST : Update product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(int id, AddUpdateProductDto updateProductDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.CategoryList = await _unitOfWork.Category.GetCategoryListAsync();
                    return View(updateProductDto);
                }

                await _unitOfWork.Products.UpdateProductAsync(id, updateProductDto);
                TempData["SuccessMessage"] = "Product successfully updated.";
                return RedirectToAction("Products");
            }
            catch (KeyNotFoundException)
            {
                _logger.LogError($"Product with id {id} not found");
                TempData["KeyNotFound"] = "Product not found";
                return RedirectToAction("Products");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the product.");
                TempData["ErrorMessage"] = "An error occurred while updating the product.";
                return RedirectToAction("Products");
            }
        }

        // POST : Delete Product
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                _logger.LogInformation($"Request to delete product with id {id}");
                await _unitOfWork.Products.DeleteProductAsync(id);
                TempData["SuccessMessage"] = "Product successfully deleted.";
                return RedirectToAction("Products");
            }
            catch (KeyNotFoundException)
            {
                _logger.LogInformation($"Product with id {id} not found.");
                TempData["KeyNotFound"] = "Product not found.";
                return RedirectToAction("Products");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting product with id {id}");
                TempData["ErrorMessage"] = "An error occurred while deleting product.";
                return RedirectToAction("Products");
            }
        }
    }
}
