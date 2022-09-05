using System.ComponentModel.DataAnnotations;

namespace OnlineMessenger.Models
{
    public class GroupMember
    {
        [Key]
        public string InvitationId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string GroupId { get; set; }

        public GroupMember(string userId, string groupId)
        {
            UserId = userId;
            GroupId = groupId;
        }

        public GroupMember(string invitationId, string userId, string groupId)
        {
            InvitationId = invitationId;
            UserId = userId;
            GroupId = groupId;
        }
    }
}
