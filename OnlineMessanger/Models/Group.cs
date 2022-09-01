namespace OnlineMessanger.Models
{
    public class Group
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool IsPublic { get; set; }
    }
}
