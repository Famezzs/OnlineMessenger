using OnlineMessenger.Models;

namespace OnlineMessenger.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> FindUserByEmail(string email);
    }
}
