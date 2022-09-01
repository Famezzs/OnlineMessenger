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

            return View(_userChats);
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
        public async Task<IActionResult> CreateChat([FromForm] Chat model)
        {
            if (!ValidateSession())
            {
                return RedirectIfUnauthorized();
            }

            var otherUser = await new UserService().FindUserByEmail(model.ParticipantBId);

            if (otherUser == null)
            {
                TempData["Error"] = "No such user exists.";

                return RedirectToAction("CreateChatForm", "Chat");
            }

            if (otherUser.Id == _userId)
            {
                TempData["Error"] = "Cannot create a chat with oneself.";

                return RedirectToAction("CreateChatForm", "Chat");
            }

            model.ParticipantAId = _userId!;

            model.ParticipantBId = otherUser.Id;

            bool isChatUnique = await new ChatService(_context!).IsUnique(model.ParticipantAId, model.ParticipantBId);

            if (!isChatUnique)
            {
                TempData["Error"] = "Chat already exists.";

                return RedirectToAction("CreateChatForm", "Chat");
            }

            await _context!.Chats!.AddAsync(model);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Chat");
        }

        public async Task<IActionResult> ViewChat(string chatId)
        {
            if (!ValidateSession())
            {
                return RedirectIfUnauthorized();
            }


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
