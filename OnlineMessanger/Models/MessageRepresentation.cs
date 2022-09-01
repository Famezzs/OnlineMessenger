namespace OnlineMessanger.Models
{
    public class MessageRepresentation
    {
        public string Id { get; set; }
        public string OwnerId { get; set; }
        public string Contents { get; set; }
        public DateTime Created { get; set; }
        public bool IsEdited { get; set; }
    }
}
