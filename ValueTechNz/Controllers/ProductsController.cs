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
            ViewBag.CategoryList = await _unitOfWork.Category.GetCategoryListAsync();
            return View();
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
                return RedirectToAction("Products", "Products");

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unable to add product.");
                return View();
            }
        }
    }
}
