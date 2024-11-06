using System.ComponentModel;

namespace ValueTechNz.Models.Dto
{
    public class LoginDto
    {
        public required string Email { get; set; } = "";
        public required string Password { get; set; } = "";

        [DisplayName("Remember me")]
        public bool rememberMe { get; set; }        
    }
}
