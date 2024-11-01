using Microsoft.AspNetCore.Mvc;
using ValueTechNz.Models.Dto;
using ValueTechNz.Models.ViewModels;
using ValueTechNz.Repository.IRepository;

namespace ValueTechNz.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IUnitOfWork unitOfWork, ILogger<ProductsController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET : Index for Product List
        public async Task<IActionResult> Products()
        {
            return View();
        }

        // GET : Populate product list
        public async Task<IActionResult> GetProducts()
        {
            var products = await _unitOfWork.Products.GetAllProductsAsync();
            return Json(products);
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

        // GET : Edit product VIEW PAGE
        public async Task<IActionResult> EditProduct(int id)
        {
            try
            {
                _logger.LogInformation($"Request to retrieve details of product with id {id}");
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

    }
}
