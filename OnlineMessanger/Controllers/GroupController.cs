using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

using OnlineMessanger.Models;
using OnlineMessanger.Helpers.Constants;
using OnlineMessanger.Helpers;
using OnlineMessanger.Services.Implementations;

namespace OnlineMessanger.Controllers
{
    public class GroupController : Controller
    {
        public async Task<IActionResult> Index()
        {
            if (!ValidateSession())
            {
                return RedirectIfUnauthorized();
            }

            _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _userGroups = await new GroupService(_context!).GetGroupsByUserId(_userId);

            return View(_userGroups);
        }

        public IActionResult CreateGroupForm()
        {
            if (!ValidateSession())
            {
                return RedirectIfUnauthorized();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromForm] Group model)
        {
            if (!ValidateSession())
            {
                return RedirectIfUnauthorized();
            }

            if (model == null ||
                String.IsNullOrWhiteSpace(model.Name))
            {
                TempData["Error"] = Constants._requiredFieldsEmptyError;

                return RedirectToAction("CreateGroupForm", "Group");
            }

            if (String.IsNullOrWhiteSpace(model.Description))
            {
                model.Description = Constants._defaultGroupDescription;
            }

            if (String.IsNullOrWhiteSpace(model.ImageUrl))
            {
                model.ImageUrl = string.Empty;
            }

            model.OwnerId = _userId!;

            await new GroupService(_context!).CreateGroup(model);

            return RedirectToAction("Index", "Group");
        }

        public async Task<IActionResult> ViewGroup()
        {
            var groupId = HttpContext.Session.GetString("GroupId");

            if (String.IsNullOrWhiteSpace(groupId))
            {
                return RedirectIfUnauthorized();
            }

            if (!ValidateSession())
            {
                return RedirectIfUnauthorized();
            }

            var isUserMemberOfGroup = new GroupService(_context!).HasAccessToGroup(_userId!, groupId);

            if (!isUserMemberOfGroup)
            {
                return RedirectIfUnauthorized();
            }

            await SetGroupName(groupId);

            _groupMessages = null;

            _defaultMessageOffset = 0;

            return await LoadNewMessages();
        }

        public async Task<IActionResult> LoadNewMessages()
        {
            var groupId = HttpContext.Session.GetString("GroupId");

            var messages = await new MessageService(_context!)
                .GetMessagesWithRepliesByChannelId(groupId!, _defaultMessageLimit, _defaultMessageOffset);

            if (_groupMessages == null)
            {
                _groupMessages = new List<MessageRepresentation>();
            }

            messages.AddRange(_groupMessages);

            _groupMessages = messages;

            _defaultMessageOffset += messages.Count;

            return View("Group", _groupMessages);
        }

        [HttpPost]
        public async Task<IActionResult> MessageHandler(string messageString)
        {
            var replyMode = HttpContext.Session.GetString("ReplyMode");

            if (String.IsNullOrWhiteSpace(replyMode))
            {
                return await SendMessage(messageString);
            }

            if (replyMode == Constants._publicReplyMode)
            {
                return await ReplyToMessage(messageString);
            }

            return await ReplyToMessagePrivately(messageString);
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
                return View("Group", _groupMessages);
            }

            var cleanMessage = TagCleaner.CleanUp(messageString);

            if (String.IsNullOrEmpty(cleanMessage))
            {
                return View("Group", _groupMessages);
            }

            var groupId = HttpContext.Session.GetString("GroupId");

            var isUserMemberOfGroup = new GroupService(_context!).HasAccessToGroup(_userId!, groupId!);

            if (!isUserMemberOfGroup)
            {
                return RedirectIfUnauthorized();
            }

            var message = new Message(_userId!, groupId!, cleanMessage, DateTime.Now);

            await new MessageService(_context!).SaveMessage(message);

            return RedirectToAction("ViewGroup", "Group");
        }

        [HttpPost]
        public async Task<IActionResult> ReplyToMessage(string messageString)
        {
            if (!ValidateSession())
            {
                return RedirectIfUnauthorized();
            }

            var messageToReplyId = HttpContext.Session.GetString("MessageReplyId");

            if (String.IsNullOrEmpty(messageString) ||
                String.IsNullOrEmpty(messageToReplyId))
            {
                return View("Group", _groupMessages);
            }

            var messageToReplyTo = _context!.Messages.Where(message => message.Id == messageToReplyId);

            if (!messageToReplyId.Any())
            {
                return View("Group", _groupMessages);
            }

            var cleanMessage = TagCleaner.CleanUp(messageString);

            if (String.IsNullOrEmpty(cleanMessage))
            {
                return View("Group", _groupMessages);
            }

            var groupId = HttpContext.Session.GetString("GroupId");

            var isUserMemberOfGroup = new GroupService(_context!).HasAccessToGroup(_userId!, groupId!);

            if (!isUserMemberOfGroup)
            {
                return RedirectIfUnauthorized();
            }

            var message = new Message(_userId!, groupId!, cleanMessage, DateTime.Now, messageToReplyId);

            await new MessageService(_context!).SaveMessage(message);

            HttpContext.Session.SetString("ReplyMode", "");

            return RedirectToAction("ViewGroup", "Group");
        }

        [HttpPost]
        public async Task<IActionResult> ReplyToMessagePrivately(string messageString)
        {
            if (!ValidateSession())
            {
                return RedirectIfUnauthorized();
            }

            var messageToReplyId = HttpContext.Session.GetString("MessageReplyId");

            if (String.IsNullOrEmpty(messageString) ||
                String.IsNullOrEmpty(messageToReplyId))
            {
                return View("Group", _groupMessages);
            }

            var messageToReplyTo = _context!.Messages.Where(message => message.Id == messageToReplyId);

            if (!messageToReplyTo.Any())
            {
                return View("Group", _groupMessages);
            }

            var cleanMessage = TagCleaner.CleanUp(messageString);

            if (String.IsNullOrEmpty(cleanMessage))
            {
                return View("Group", _groupMessages);
            }

            var groupId = HttpContext.Session.GetString("GroupId");

            var isUserMemberOfGroup = new GroupService(_context!).HasAccessToGroup(_userId!, groupId!);

            if (!isUserMemberOfGroup)
            {
                return RedirectIfUnauthorized();
            }

            var chatId = await new ChatService(_context!).CreateChatIfNotExists(_userId!, messageToReplyTo.First().OwnerId);

            var message = new Message(_userId!, chatId, cleanMessage, DateTime.Now, messageToReplyId);

            await new MessageService(_context!).SaveMessage(message);

            HttpContext.Session.SetString("ReplyMode", "");

            HttpContext.Session.SetString("ChatId", chatId);

            return RedirectToAction("ViewChat", "Chat");
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

            return RedirectToAction("ViewGroup", "Group");
        }

        [HttpPost]
        public async Task<string?> Invite(string email)
        {
            if (String.IsNullOrWhiteSpace(email))
            {
                return Constants._requiredFieldsEmptyError;
            }

            var groupId = HttpContext.Session.GetString("GroupId");

            try
            {
                await new GroupService(_context!).InviteToGroup(email, groupId!);
            }
            catch (Exception exception)
            {
                return exception.Message;
            }

            return null;
        }

        [HttpPost]
        public async Task<string?> RemoveMember(string email)
        {
            if (String.IsNullOrWhiteSpace(email))
            {
                return Constants._requiredFieldsEmptyError;
            }

            var groupId = HttpContext.Session.GetString("GroupId");

            try
            {
                await new GroupService(_context!).RemoveFromGroup(email, groupId!, _userId!);
            }
            catch (Exception exception)
            {
                return exception.Message;
            }

            return null;
        }

        [HttpPost]
        public void SetGroupId(string groupId)
        {
            HttpContext.Session.SetString("GroupId", groupId);

            var groupOwner = _userGroups!.Find(group => group.Id == groupId)!.OwnerId;

            HttpContext.Session.SetString("GroupOwner", groupOwner);
        }

        [HttpPost]
        public string SetMessageId(string messageId)
        {
            HttpContext.Session.SetString("MessageId", messageId);

            return _groupMessages!.Find(message => message.Message.Id == messageId)!.Message.Contents;
        }

        [HttpPost]
        public void SetReplyMode(string messageId, string replyMode)
        {
            HttpContext.Session.SetString("MessageReplyId", messageId);

            HttpContext.Session.SetString("ReplyMode", replyMode);
        }

        [HttpGet]
        public string GetMembersOfGroup()
        {
            var groupId = HttpContext.Session.GetString("GroupId");

            return new GroupService(_context!).GetMembersByGroupId(groupId!);
        }

        public async Task SetGroupName(string groupId)
        {
            var group = await _context!.Groups.FindAsync(groupId);

            if (group != null)
            {
                HttpContext.Session.SetString("GroupName", group.Name);
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

        public GroupController(MessangerDataContext context)
        {
            _context = context;
        }

        private static string? _userId;

        private static List<Group>? _userGroups;

        private static MessangerDataContext? _context;

        private static List<MessageRepresentation>? _groupMessages;

        private static int _defaultMessageOffset = 0;

        private static int _defaultMessageLimit = 20;
    }
}
