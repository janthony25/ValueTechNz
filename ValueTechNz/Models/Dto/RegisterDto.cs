using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ValueTechNz.Models.Dto
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "First name is required."), MaxLength(100)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required."), MaxLength(100)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [EmailAddress, MaxLength(200)]
        public required string Email { get; set; }

        [Phone(ErrorMessage = "Phone number format is not valid."), MaxLength(20)]
        [DisplayName("Contact #")]
        public string? PhoneNumber { get; set; }

        [MaxLength(200)]
        public required string Address { get; set; }

        [MaxLength(200)]
        public required string Password { get; set; }

        [Required(ErrorMessage = "The confirm password field is required.")]
        [Compare("Password", ErrorMessage = "Confirim password and password does not match.")]
        public string ConfirmPassword { get; set; } 


    }
}
