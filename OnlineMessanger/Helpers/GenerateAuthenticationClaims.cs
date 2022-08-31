using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OnlineMessanger.Helpers
{
    public class GenerateAuthenticationClaims
    {
        public static List<Claim> Generate(string claimType, string claimValue)
        {
            return new List<Claim>
                {
                    new Claim(claimType, claimValue),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
        }
    }
}
