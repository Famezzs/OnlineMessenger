using OnlineMessenger.Models;

namespace OnlineMessenger.Services.Interfaces
{
    public interface IChatService
    {
        Task<List<ChatRepresentation>> GetChatsByUserId(string userId);
        Task<bool> IsChatUnique(string firstUserId, string secondUserId);
        Task<bool> HasAccessToChat(string userId, string chatId);
        Task<string> CreateChatIfNotExists(string firstUserId, string secondUserId);
    }
}
