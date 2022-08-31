using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using OnlineMessanger.Helpers;
using OnlineMessanger.Helpers.Constants;
using OnlineMessanger.Models;

namespace OnlineMessanger.Controllers
{
    public class AuthenticateController : Controller
    {

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromForm] Login model)
        {
            var user = await userManager.FindByNameAsync(model.Email);

            var isAuthenticationAttemptValid = user != null && await userManager.CheckPasswordAsync(user, model.Password);

            if (isAuthenticationAttemptValid == false)
            {
                TempData["Error"] = Constants._loginFailMessage;
                return RedirectToAction("Login", "Home");
            }

            var userRoles = await userManager.GetRolesAsync(user!);

            var authenticationClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user!.Email),
                new Claim(ClaimTypes.Name, user!.UserName),
                new Claim(ClaimTypes.NameIdentifier, user!.Id)
            };

            foreach (var userRole in userRoles)
            {
                authenticationClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var securityToken = GenerateSecurityToken
                .Generate(authenticationClaims, _configuration);

            HttpContext.Session.SetString("Token", new JwtSecurityTokenHandler().WriteToken(securityToken));

            await signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromForm] Register model)
        {
            var applicationUser = new User()
            {
                Email = model.Email,
                UserName = model.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await userManager.CreateAsync(applicationUser, model.Password);

            if (result.Succeeded)
            {
                return await Login(new Login()
                {
                    Email = model.Email,
                    Password = model.Password
                });
            }
            else
            {
                var errorMessage = string.Empty;

                foreach (var error in result.Errors)
                {
                    errorMessage += error.Description + ' ';
                }

                TempData["Error"] = errorMessage;
                return RedirectToAction("Register", "Home");
            }    
        }

        public AuthenticateController(UserManager<User> userManager, 
            IConfiguration configuration,
            SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _configuration = configuration;
        }

        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IConfiguration _configuration;
    }
}