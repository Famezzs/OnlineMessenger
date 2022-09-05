using System.ComponentModel.DataAnnotations;

namespace OnlineMessenger.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public Login()
        {
            Email = String.Empty;
            Password = String.Empty;
        }
    }
}
