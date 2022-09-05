using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace OnlineMessanger.Helpers
{
    public class GenerateSecurityToken
    {
        public static JwtSecurityToken Generate(List<Claim> authenticationClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")));

            return new JwtSecurityToken
                (
                    issuer: Environment.GetEnvironmentVariable("JWT_VALID_ISSUER"),
                    audience: Environment.GetEnvironmentVariable("JWT_VALID_AUDIENCE"),
                    expires: DateTime.Now.AddMinutes(30),
                    claims: authenticationClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
        }
    }
}
