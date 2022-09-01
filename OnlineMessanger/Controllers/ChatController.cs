using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

using OnlineMessanger.Models;
using OnlineMessanger.Services;

namespace OnlineMessanger.Controllers
{
    public class ChatController : Controller
    {
        public async Task<IActionResult> Index()
        {
            if (!ValidateSession())
            {
                return RedirectIfUnauthorized();
            }

            _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _userChats = await new ChatService(_context!).GetChatsByUserId(_userId);

            return View();
        }

        public IActionResult CreateChatForm()
        {
            if (!ValidateSession())
            {
                return RedirectIfUnauthorized();
            }

            return View();
        }

        [HttpPost]
        public IActionResult CreateChat([FromForm] Chat model)
        {
            if (!ValidateSession())
            {
                return RedirectIfUnauthorized();
            }

            var otherUser = await _context.FindAsync()
        }

        private bool ValidateSession()
        {
            var token = HttpContext.Session.GetString("Token");

            return new TokenService().IsTokenValid(token);
        }

        private IActionResult RedirectIfUnauthorized()
        {
            return RedirectToAction("Login", "Home");
        }

        public ChatController(MessangerDataContext context)
        {
            _context = context;
        }

        private static string? _userId;

        private static List<ChatRepresentation>? _userChats;

        private static MessangerDataContext? _context;
    }
}
