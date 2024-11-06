using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NPOI.OpenXmlFormats.Dml.Chart;
using ValueTechNz.Models;
using ValueTechNz.Models.Dto;
using ValueTechNz.Repository.IRepository;

namespace ValueTechNz.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountRepository> _logger;

        public AccountRepository(UserManager<ApplicationUser> userManager, 
                                             SignInManager<ApplicationUser> signInManager,
                                             ILogger<AccountRepository> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                // Create new user object from registration data
                var user = new ApplicationUser
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    PhoneNumber = registerDto.PhoneNumber,
                    Address = registerDto.Address
                };

                // Attempt to create the user
                var result = await _userManager.CreateAsync(user, registerDto.Password);

                // If creation failed, throw exception with details
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.FirstOrDefault().Description ?? "Registration failed.");
                }

                // Add user client role
                var roleResult = await _userManager.AddToRoleAsync(user, "client");
                if (!roleResult.Succeeded)
                {
                    throw new Exception("Failed to assign role");
                }

                // Sign in the user
                await _signInManager.SignInAsync(user, false);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing user registration.");
                throw;
            }
        }
    }
}
