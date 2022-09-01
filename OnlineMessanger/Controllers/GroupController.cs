using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

using OnlineMessanger.Services;
using OnlineMessanger.Models;

namespace OnlineMessanger.Controllers
{
    public class GroupController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("Token");

            if (!new TokenService().IsTokenValid(token))
            {
                return RedirectToAction("Login", "Home");
                
            }

            _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _userGroups = await new GroupService(_context!).GetGroupsByUserId(_userId);

            return View();
        }

        public GroupController(MessangerDataContext context)
        {
            _context = context;
        }

        private static string? _userId;

        private static List<Group>? _userGroups;

        private static MessangerDataContext? _context;
    }
}
