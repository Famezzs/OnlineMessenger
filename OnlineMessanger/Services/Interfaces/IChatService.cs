using OnlineMessanger.Models;

namespace OnlineMessanger.Services.Interfaces
{
    public interface IChatService
    {
        Task<List<ChatRepresentation>> GetChatsByUserId(string userId);
    }
}
