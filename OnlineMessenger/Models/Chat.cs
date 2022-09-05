namespace OnlineMessenger.Models
{
    public class Chat
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ParticipantAId { get; set; }
        public string ParticipantBId { get; set; }

        public Chat()
        {
            ParticipantAId = string.Empty;
            ParticipantBId = string.Empty;
        }

        public Chat(string participantAId, string participantBId)
        {
            ParticipantAId = participantAId;
            ParticipantBId = participantBId;
        }

        public Chat(string id, string participantAId, string participantBId)
        {
            Id = id;
            ParticipantAId = participantAId;
            ParticipantBId = participantBId;
        }
    }
}
