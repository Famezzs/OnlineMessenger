using OnlineMessenger.Helpers;
using OnlineMessenger.Models;
using OnlineMessenger.Services.Interfaces;

namespace OnlineMessenger.Services.Implementations
{
    public class ChatService : IChatService
    {
        public async Task<List<ChatRepresentation>> GetChatsByUserId(string userId)
        {
            var chats = new List<ChatRepresentation>();

            var fields = "Id, ParticipantAId, ParticipantBId";

            var source = "dbo.Chats";

            var condition = $"ParticipantAId='{userId}' OR ParticipantBId='{userId}'";

            using var queryService = new QueryService();

            var sqlReader = await queryService.Select(fields, source, condition);

            while (await sqlReader.ReadAsync())
            {
                var chatId = (string)sqlReader["Id"];

                var participantAId = (string)sqlReader["ParticipantAId"];

                var participantBId = (string)sqlReader["ParticipantBId"];

                var chatName = string.Empty;

                var secondUser = new User();

                if (participantAId == userId)
                {
                    secondUser = await context.Users.FindAsync(participantBId);
                }
                else
                {
                    secondUser = await context.Users.FindAsync(participantAId);
                }

                chatName = secondUser!.Email;

                chats.Add(new ChatRepresentation(chatName, new Chat(chatId, participantAId, participantBId)));
            }

            return chats;
        }

        public async Task<bool> IsChatUnique(string firstUserId, string secondUserId)
        {
            var fields = "Id";

            var source = "dbo.Chats";

            var condition = $"ParticipantAId='{firstUserId}' AND ParticipantBId='{secondUserId}'" +
                            $"OR ParticipantAId='{secondUserId}' AND ParticipantBId='{firstUserId}'";

            using var queryService = new QueryService();

            var sqlReader = await queryService.Select(fields, source, condition);

            if (await sqlReader.ReadAsync())
            {
                return false;
            }

            return true;
        }

        public async Task<bool> HasAccessToChat(string userId, string chatId)
        {
            var chat = await context!.Chats!.FindAsync(chatId);

            if (chat == null)
            {
                return false;
            }

            if (chat.ParticipantAId != userId &&
                chat.ParticipantBId != userId)
            {
                return false;
            }

            return true;
        }

        public async Task<string> CreateChatIfNotExists(string participantAId, string participantBId)
        {
            var participantIds = new HashSet<string>();

            participantIds.Add(participantAId);

            participantIds.Add(participantBId);

            var existingChat = context.Chats.Where(chat => participantIds.Contains(chat.ParticipantAId) &&
                                                    participantIds.Contains(chat.ParticipantBId));

            if (existingChat.Any())
            {
                return existingChat.First().Id;
            }

            var newChat = new Chat(participantAId, participantBId);

            await context.Chats.AddAsync(newChat);

            await context.SaveChangesAsync();

            return newChat.Id;
        }

        public ChatService(MessengerDataContext context)
        {
            this.context = context;
        }

        private MessengerDataContext context;
    }
}
