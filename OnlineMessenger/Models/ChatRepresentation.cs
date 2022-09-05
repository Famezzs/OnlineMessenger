namespace OnlineMessenger.Models
{
    public class ChatRepresentation
    {
        public string Name { get; set; }
        public Chat Chat { get; set; }

        public ChatRepresentation(string name, Chat chat)
        {
            Name = name;
            Chat = chat;
        }
    }
}
