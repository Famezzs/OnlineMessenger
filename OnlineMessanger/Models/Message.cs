namespace OnlineMessanger.Models
{
    public class Message
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string OwnerId { get; set; }
        public string ChannelId { get; set; }
        public string Contents { get; set; }
        public DateTime Created { get; set; }
        public bool IsEdited { get; set; }
        public bool IsDeletedForSelf { get; set; }

        public Message()
        {
            OwnerId = string.Empty;
            ChannelId = string.Empty;
            Contents = string.Empty;
        }

        public Message(string id, string ownerId, string channelId, string contents, DateTime created, bool isEdited = false, bool isDeletedForSelf = false)
        {
            Id = id;
            OwnerId = ownerId;
            ChannelId = channelId;
            Contents = contents;
            Created = created;
            IsEdited = isEdited;
            IsDeletedForSelf = isDeletedForSelf;
        }

        public Message(string ownerId, string channelId, string contents, DateTime created, bool isEdited = false, bool isDeletedForSelf = false)
        {
            OwnerId = ownerId;
            ChannelId = channelId;
            Contents = contents;
            Created = created;
            IsEdited = isEdited;
            IsDeletedForSelf = isDeletedForSelf;
        }
    }
}
