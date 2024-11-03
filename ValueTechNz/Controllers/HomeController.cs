using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ValueTechNz.Models;
using ValueTechNz.Repository.IRepository;

namespace ValueTechNz.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var latestProducts = await _unitOfWork.Products.GetLatestProductsAsync();
                return View(latestProducts);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching latest products.");
                TempData["ErrorMessage"] = "An error occurred while fetching latest products.";
                return View("Index");
            }
            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
