using OnlineMessanger.Models;

namespace OnlineMessanger.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user, IList<string> userRoles, DateTime expiresIn);

        bool IsTokenValid(string token);
    }
}
