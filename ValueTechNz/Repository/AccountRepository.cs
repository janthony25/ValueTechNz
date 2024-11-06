using Microsoft.AspNetCore.Identity;
using ValueTechNz.Models;
using ValueTechNz.Repository.IRepository;

namespace ValueTechNz.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


    }
}
