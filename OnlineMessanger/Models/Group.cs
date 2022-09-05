using OnlineMessanger.Helpers.Constants;

namespace OnlineMessanger.Models
{
    public class Group
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string OwnerId { get; set; }

        public Group()
        {
            Name = string.Empty;
            Description = Constants._defaultGroupDescription;
            ImageUrl = string.Empty;
            OwnerId = string.Empty;
        }

        public Group(string name, string description, string imageUrl, string ownerId)
        {
            Name = name;
            Description = description;
            ImageUrl = imageUrl;
            OwnerId = ownerId;
        }

        public Group(string id, string name, string description, string imageUrl, string ownerId)
        {
            Id = id;
            Name = name;
            Description = description;
            ImageUrl = imageUrl;
            OwnerId = ownerId;
        }
    }
}
