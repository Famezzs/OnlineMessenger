﻿using Microsoft.AspNetCore.Mvc;

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

        public async Task<IActionResult> ViewChat()
        {
            var chatId = HttpContext.Session.GetString("ChatId");

            if (String.IsNullOrWhiteSpace(chatId))
            {
                return RedirectIfUnauthorized();
            }

            if (!ValidateSession())
            {
                return RedirectIfUnauthorized();
            }

            var doesUserHaveAccessToChat = await new ChatService(_context!).HasAccessToChat(_userId!, chatId);

            if (!doesUserHaveAccessToChat)
            {
                return RedirectIfUnauthorized();
            }

            var messages = await new ChatService(_context!).GetMessagesByChatId(chatId, _messageLimit, _messageOffset);

            _chatMessages.AddRange(messages);

            _messageOffset += _messageLimit;

            return View("Chat", _chatMessages);
        }

        [HttpPost]
        public void SetChatId(string chatId)
        {
            HttpContext.Session.SetString("ChatId", chatId);
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

        private static List<Message> _chatMessages = new List<Message>();

        private static MessangerDataContext? _context;

        private static int _messageOffset = 0;

        private static int _messageLimit = 20;
    }
}
