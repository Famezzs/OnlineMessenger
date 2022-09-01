using OnlineMessanger.Models;
using OnlineMessanger.Services.Interfaces;

namespace OnlineMessanger.Services
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

        public async Task<List<Message>> GetMessagesByChatId(string chatId, int limit, int offset)
        {
            var messages = new List<Message>();

            var fields = $"*";

            var source = "dbo.Messages";

            var condition = $"ChannelId='{chatId}'";

            var additionalOptions = $"ORDER BY Created OFFSET {offset} ROWS FETCH FIRST {limit} ROWS ONLY";

            using var queryService = new QueryService();

            var sqlReader = await queryService.Select(fields, source, condition, additionalOptions);

            while (await sqlReader.ReadAsync())
            {
                var id = (string)sqlReader["Id"];

                var ownerId = (string)sqlReader["OwnerId"];

                var channelId = (string)sqlReader["ChannelId"];

                var contents = (string)sqlReader["Contents"];

                var createdOn = (DateTime)sqlReader["Created"];

                var isEdited = (bool)sqlReader["IsEdited"];

                messages.Add(new Message(id, ownerId, channelId, contents, createdOn, isEdited));
            }

            return messages;
        }

        public async Task<bool> IsUnique(string firstUserId, string secondUserId)
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

        public ChatService(MessangerDataContext context)
        {
            this.context = context;
        }

        private MessangerDataContext context;
    }
}
