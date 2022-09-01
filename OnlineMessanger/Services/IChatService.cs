using OnlineMessanger.Models;

namespace OnlineMessanger.Services
{
    public interface IChatService
    {
        Task<List<ChatRepresentation>> GetChatsByUserId(string userId);
    }
}
