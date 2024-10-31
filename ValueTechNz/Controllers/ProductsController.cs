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
            var categories = await _unitOfWork.Category.GetCategoryListAsync();

            var viewModel = new ProductViewModel
            {
                Product = new AddUpdateProductDto
                {
                    ProductName = string.Empty,
                    Brand = string.Empty,
                    Price = 0,
                    CategoryId = 0,
                    CategoryName = string.Empty
                },
                CategoryList = categories
            };

            return View(viewModel);
        }

       
    }
}
