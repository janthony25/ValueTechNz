using Microsoft.AspNetCore.Mvc;
using ValueTechNz.Repository.IRepository;

namespace ValueTechNz.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }
    }
}
