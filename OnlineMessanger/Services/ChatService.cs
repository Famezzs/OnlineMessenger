using Microsoft.Data.SqlClient;

using OnlineMessanger.Helpers;
using OnlineMessanger.Models;

namespace OnlineMessanger.Services
{
    public class ChatService : IChatService
    {
        public async Task<List<ChatRepresentation>> GetChatsByUserId(string userId)
        {

            var chats = new List<ChatRepresentation>();

            using (var sqlConnection = new SqlConnection(ConnectionStrings.GetSqlConnectionString()))
            {
                await sqlConnection.OpenAsync();

                using var sqlCommand = 
                    new SqlCommand($"SELECT Id, ParticipantAId, ParticipantBId FROM dbo.Chats WHERE ParticipantAId='{userId}' OR ParticipantBId='{userId}'", sqlConnection);

                using var reader = await sqlCommand.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var chatId = (string)reader["Id"];

                    var participantAId = (string)reader["ParticipantAId"];

                    var participantBId = (string)reader["ParticipantBId"];

                    var chatName = string.Empty;

                    if (participantAId == userId)
                    {
                        var secondUser = await context.Users.FindAsync(participantBId);

                        chatName = secondUser!.Email;
                    }
                    else
                    {
                        var user = await context.Users.FindAsync(userId);

                        chatName = user!.Email;
                    }

                    chats.Add(new ChatRepresentation(chatId, chatName));
                }
            }

            return chats;
        }

        public ChatService(MessangerDataContext context)
        {
            this.context = context;
        }

        private MessangerDataContext context;
    }
}
