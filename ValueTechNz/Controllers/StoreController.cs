using Microsoft.AspNetCore.Mvc;
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

        public async  Task<IActionResult> Index(int pageNumber = 1, string search = null)
        {
            try
            {
                ViewData["CurrentSearch"] = search;
                var products = await _unitOfWork.Store.GetStoreProductsAsync(pageNumber,
                                                                             pageSize,
                                                                             search);

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
