using Microsoft.AspNetCore.Identity;

namespace ValueTechNz.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
