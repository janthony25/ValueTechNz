using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NPOI.OpenXmlFormats.Dml.Chart;
using ValueTechNz.Models;
using ValueTechNz.Models.Dto;
using ValueTechNz.Repository.IRepository;

namespace ValueTechNz.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountRepository accountRepository, ILogger<AccountController> logger, SignInManager<ApplicationUser> signInManager)
        {
            _accountRepository = accountRepository;
            _logger = logger;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }

        // POST : Submit Registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                   foreach(var modelStateEntry in ModelState.Values)
                    {
                        foreach(var error in modelStateEntry.Errors)
                        {

                            _logger.LogError($"Validation error: {error.ErrorMessage}");
                        }
                    }
                    return View(registerDto);
                }


                await _accountRepository.RegisterAsync(registerDto);
                TempData["SuccessMessage"] = "Registration successfull!";
                return RedirectToAction("Index", "Home");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to process user registration.");
                TempData["ErrorMessage"] = "An error occurred while trying to process your registration.";
                return RedirectToAction("Register");
            }
        }

        // GET: Logout User
        public async Task<IActionResult> Logout()
        {
            try
            {
                if (_signInManager.IsSignedIn(User))
                {
                    await _signInManager.SignOutAsync();
                }
                return RedirectToAction("Index", "Home");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to logout.");
                TempData["ErrorMessage"] = "An error occurred while trying to logout";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
