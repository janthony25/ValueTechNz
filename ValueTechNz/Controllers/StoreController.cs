using Microsoft.AspNetCore.Mvc;
using NPOI.OpenXmlFormats.Dml.Diagram;
using ValueTechNz.Models;
using ValueTechNz.Repository.IRepository;

namespace ValueTechNz.Controllers
{
    
    public class StoreController : Controller
    {
        private readonly ILogger<StoreController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private const int pageSize = 12;

        public StoreController(ILogger<StoreController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string search = null,
                                               string brand = null, string category = null, string sort = null)
        {
            try
            {
                // Store current filter values in ViewData to maintain state
                ViewData["CurrentSearch"] = search;
                ViewBag.CurrentBrand = brand;
                ViewBag.CurrentCategory = category;
                ViewBag.CurrentSort = sort;


                ViewBag.Categories = await _unitOfWork.Category.GetCategoryListAsync();

                var products = await _unitOfWork.Store.GetStoreProductsAsync(
                    pageNumber,
                    pageSize,
                    search,
                    brand,
                    category,
                    sort
                );


                return View(products);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching product list");
                throw;
            }
        }
    }
}
