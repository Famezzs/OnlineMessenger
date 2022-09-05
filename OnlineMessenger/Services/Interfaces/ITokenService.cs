using OnlineMessenger.Models;

namespace OnlineMessenger.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user, IList<string> userRoles, DateTime expiresIn);
        bool IsTokenValid(string token);
    }
}
