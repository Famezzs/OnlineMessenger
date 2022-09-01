﻿namespace OnlineMessanger.Models
{
    public class Group
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string OwnerId { get; set; }
        public bool IsPublic { get; set; }
    }
}
