using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnlineMessanger.Services;

namespace OnlineMessanger.Controllers
{
    public class GroupController : Controller
    {
        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("Token");

            if (new TokenService().IsTokenValid(token))
            {
                return View();
            }

            return RedirectToAction("Login", "Home");
        }
    }
}
