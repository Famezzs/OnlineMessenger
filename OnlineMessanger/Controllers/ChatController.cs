using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

using OnlineMessanger.Models;
using OnlineMessanger.Helpers;
using OnlineMessanger.Helpers.Constants;
using OnlineMessanger.Services.Implementations;

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
                TempData["Error"] = Constants._noSuchUserExistsError;

                return RedirectToAction("CreateChatForm", "Chat");
            }

            if (otherUser.Id == _userId)
            {
                TempData["Error"] = Constants._cannotCreateChatWithSelfError;

                return RedirectToAction("CreateChatForm", "Chat");
            }

            model.ParticipantAId = _userId!;

            model.ParticipantBId = otherUser.Id;

            bool isChatUnique = await new ChatService(_context!).IsChatUnique(model.ParticipantAId, model.ParticipantBId);

            if (!isChatUnique)
            {
                TempData["Error"] = Constants._chatAlreadyExistsError;

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

            _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var doesUserHaveAccessToChat = await new ChatService(_context!).HasAccessToChat(_userId!, chatId);

            if (!doesUserHaveAccessToChat)
            {
                return RedirectIfUnauthorized();
            }

            await SetChatName(chatId);

            _chatMessages = null;

            _defaultMessageOffset = 0;

            return await LoadNewMessages();
        }

        public async Task<IActionResult> LoadNewMessages()
        {
            var chatId = HttpContext.Session.GetString("ChatId");

            var messages = await new MessageService(_context!)
                .GetMessagesWithRepliesByChannelId(chatId!, _defaultMessageLimit, _defaultMessageOffset);

            if (_chatMessages == null)
            {
                _chatMessages = new List<MessageRepresentation>();
            }

            messages.AddRange(_chatMessages);

            _chatMessages = messages;

            _defaultMessageOffset += messages.Count;

            return View("Chat", _chatMessages);
        }

        [HttpPost]
        public async Task DeleteMessage(string messageId)
        {
            await new MessageService(_context!).DeleteMessage(_userId!, messageId);
        }

        [HttpPost]
        public async Task DeleteMessageForSelf(string messageId)
        {
            await new MessageService(_context!).DeleteMessageForSelf(_userId!, messageId);
        }

        [HttpPost]
        public async Task<IActionResult> EditMessage(string newContents)
        {
            var messageId = HttpContext.Session.GetString("MessageId");

            await new MessageService(_context!).EditMessage(_userId!, messageId!, newContents);

            return RedirectToAction("ViewChat", "Chat");
        }

        [HttpPost]
        public void SetChatId(string chatId)
        {
            HttpContext.Session.SetString("ChatId", chatId);
        }

        [HttpPost]
        public string SetMessageId(string messageId)
        {
            HttpContext.Session.SetString("MessageId", messageId);

            return _chatMessages!.Find(message => message.Message.Id == messageId)!.Message.Contents;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string messageString)
        {
            if (!ValidateSession())
            {
                return RedirectIfUnauthorized();
            }

            if (String.IsNullOrEmpty(messageString))
            {
                return View("Chat", _chatMessages);
            }

            var cleanMessage = TagCleaner.CleanUp(messageString);

            if (String.IsNullOrEmpty(cleanMessage))
            {
                return View("Chat", _chatMessages);
            }

            var chatId = HttpContext.Session.GetString("ChatId");

            var doesUserHaveAccessToChat = await new ChatService(_context!).HasAccessToChat(_userId!, chatId!);

            if (!doesUserHaveAccessToChat)
            {
                return RedirectIfUnauthorized();
            }

            var message = new Message(_userId!, chatId!, cleanMessage, DateTime.Now);

            await new MessageService(_context!).SaveMessage(message);

            return RedirectToAction("ViewChat", "Chat");
        }

        private async Task SetChatName(string chatId)
        {
            var chat = await _context!.Chats.FindAsync(chatId);

            if (chat == null)
            {
                return;
            }

            var userA = await _context!.Users.FindAsync(chat.ParticipantAId);

            if (userA == null)
            {
                return;
            }

            var userB = await _context!.Users.FindAsync(chat.ParticipantBId);

            if (userB == null)
            {
                return;
            }

            if (_userId == userA.Id)
            {
                HttpContext.Session.SetString("ChatName", userB.Email);
            }
            else
            {
                HttpContext.Session.SetString("ChatName", userA.Email);
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

        private static string _userId = string.Empty;

        private static List<ChatRepresentation>? _userChats;

        private static List<MessageRepresentation>? _chatMessages;

        private static MessangerDataContext? _context;

        private static int _defaultMessageOffset = 0;

        private static int _defaultMessageLimit = 20;
    }
}
