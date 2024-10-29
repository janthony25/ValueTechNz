using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Products()
        {
            return View();
        }

        public async Task<IActionResult> GetProducts()
        {
            var products = await _unitOfWork.Products.GetAllProductsAsync();
            return Json(products);
        }
    }
}
