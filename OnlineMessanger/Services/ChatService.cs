using Microsoft.Data.SqlClient;

using OnlineMessanger.Helpers;
using OnlineMessanger.Models;
using OnlineMessanger.Services.Interfaces;

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
                        var secondUser = await context.Users.FindAsync(participantAId);

                        chatName = secondUser!.Email;
                    }

                    chats.Add(new ChatRepresentation(chatId, chatName));
                }
            }

            return chats;
        }

        public async Task<bool> IsUnique(string firstUserId, string secondUserId)
        {
            using (var sqlConnection = new SqlConnection(ConnectionStrings.GetSqlConnectionString()))
            {
                await sqlConnection.OpenAsync();

                using var sqlCommand = new SqlCommand($"SELECT Id FROM dbo.Chats WHERE ParticipantAId='{firstUserId}' AND ParticipantBId='{secondUserId}'" +
                                    $"OR ParticipantAId='{secondUserId}' AND ParticipantBId='{firstUserId}'", sqlConnection);

                using var reader = await sqlCommand.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return false;
                }
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
