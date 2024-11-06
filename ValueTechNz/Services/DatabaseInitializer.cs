using Microsoft.AspNetCore.Identity;
using ValueTechNz.Models;

namespace ValueTechNz.Services
{
    public class DatabaseInitializer
    {
        public static async Task SeedDataAsync(UserManager<ApplicationUser>? userManager,
                                    RoleManager<IdentityRole>? roleManager)
        {
            if(userManager == null || roleManager == null)
            {
                Console.WriteLine("userManager or roleManager is null => exit");
                return;
            }

            // Check if admin role exist
            var exists = await roleManager.RoleExistsAsync("admin");
            if (!exists)
            {
                Console.WriteLine("Admin role is not defined and will be created.");
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }

            // Check if seller role exist
            exists = await roleManager.RoleExistsAsync("seller");
            if (!exists)
            {
                Console.WriteLine("Seller role is not defined and will be created.");
                await roleManager.CreateAsync(new IdentityRole("seller"));
            }

            // Check if client role exist
            exists = await roleManager.RoleExistsAsync("client");
            if (!exists)
            {
                Console.WriteLine("Client role is not defined and will be created.");
                await roleManager.CreateAsync(new IdentityRole("client"));
            }

            // Check if atleast 1 admin user exist
            var adminUsers = await userManager.GetUsersInRoleAsync("admin");
            if (adminUsers.Any())
            {
                // Admin user already exist
                Console.WriteLine("Admin user already exist => exit");
                return;
            }

            // Create the admin user
            var user = new ApplicationUser()
            {
                FirstName = "Admin",
                LastName = "Admin",
                UserName = "junanthony.business@gmail.com", // Will be used to authenticate user
                Email = "junanthony.business@gmail.com"
            };

            string initialPassword = "@Admin123!";

            var result = await userManager.CreateAsync(user, initialPassword);
            Console.WriteLine("Attempting to create admin user...");
            if (result.Succeeded)
            {
                // Set the user role
                await userManager.AddToRoleAsync(user, "admin");
                Console.WriteLine("Admin user successfully created! Please update initial password");
                Console.WriteLine($"Email: {user.Email}");
                Console.WriteLine($"Initial password: {initialPassword}");
            }
            else
            {
                Console.WriteLine("Failed to create admin user. Errors:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"- {error.Description}");
                }
            }

        }
    }
}
