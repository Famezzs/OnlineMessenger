using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;
using OnlineMessanger.Helpers;
using OnlineMessanger.Models;

namespace OnlineMessanger.Services
{
    public class TokenService : ITokenService
    {
        public string CreateToken(User user, IList<string> userRoles, DateTime expiresIn)
        {
            var authenticationClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var userRole in userRoles)
            {
                authenticationClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenCredentials.GetSecurityKey()));

            var issuer = TokenCredentials.GetIssuer();

            var audience = TokenCredentials.GetAudience();

            var securityToken =  new JwtSecurityToken
                (
                    issuer: issuer,
                    audience: audience,
                    expires: expiresIn,
                    claims: authenticationClaims,
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        public bool IsTokenValid(string? token)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenCredentials.GetSecurityKey()));

            var issuer = TokenCredentials.GetIssuer();

            var audience = TokenCredentials.GetAudience();

            try 
            {
                jwtTokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = signingKey
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
