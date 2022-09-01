namespace OnlineMessanger.Models
{
    public class ChatRepresentation
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public ChatRepresentation(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
