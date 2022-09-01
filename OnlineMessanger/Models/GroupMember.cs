using System.ComponentModel.DataAnnotations;

namespace OnlineMessanger.Models
{
    public class GroupMember
    {
        [Key]
        public string InvitationId { get; set; }
        public string UserId { get; set; }
        public string GroupId { get; set; }
    }
}
