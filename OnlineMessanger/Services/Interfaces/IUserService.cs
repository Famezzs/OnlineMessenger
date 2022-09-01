using OnlineMessanger.Models;

namespace OnlineMessanger.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> FindUserByEmail(string email);
    }
}
