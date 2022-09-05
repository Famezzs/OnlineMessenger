using OnlineMessanger.Models;

namespace OnlineMessanger.Services.Interfaces
{
    public interface IMessageService
    {
        public Task SaveMessage(Message message);
        public Task EditMessage(string userId, string messageId, string contents);
        public Task DeleteMessage(string userId, string messageId);
        public Task DeleteMessageForSelf(string userId, string messageId);
        public Task<List<MessageRepresentation>> GetMessagesByChannelId(string channelId, int messageLimit, int messageOffset);
        Task<List<MessageRepresentation>> GetMessagesWithRepliesByChannelId(string channelId, int messageLimit, int messageOffset);
        public Task<bool> IsUserOwnerOfMessage(string userId, string messageId);
    }
}
