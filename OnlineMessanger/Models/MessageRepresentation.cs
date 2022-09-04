namespace OnlineMessanger.Models
{
    public class MessageRepresentation
    {
        public Message Message { get; set; }
        public Message? ReplyTo { get; set; }
        public string OwnerName { get; set; }

        public MessageRepresentation(Message message, string ownerName)
        {
            Message = message;
            OwnerName = ownerName;
        }

        public MessageRepresentation(Message message, Message? replyTo, string ownerName)
        {
            Message = message;
            ReplyTo = replyTo;
            OwnerName = ownerName;
        }
    }
}
