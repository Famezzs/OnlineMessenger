using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace OnlineMessenger.Models
{
    public class User : IdentityUser
    {
        public User()
        {

        }

        public User(string userName, string email)
        {
            UserName = userName;
            Email = email;
        }
    }
}
