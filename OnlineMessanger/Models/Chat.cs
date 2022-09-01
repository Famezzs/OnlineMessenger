namespace OnlineMessanger.Models
{
    public class Chat
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ParticipantAId { get; set; }
        public string ParticipantBId { get; set; }
    }
}
