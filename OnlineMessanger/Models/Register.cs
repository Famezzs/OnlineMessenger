using System.ComponentModel.DataAnnotations;

namespace OnlineMessanger.Models
{
    public class Register
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public Register()
        {
            Email = String.Empty;
            Password = String.Empty;
        }

        public Register(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
