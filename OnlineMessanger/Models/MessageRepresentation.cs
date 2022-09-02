namespace OnlineMessanger.Models
{
    public class MessageRepresentation
    {
        public Message Message { get; set; }
        public string OwnerName { get; set; }

        public MessageRepresentation(Message message, string ownerName)
        {
            Message = message;
            OwnerName = ownerName;
        }
    }
}
