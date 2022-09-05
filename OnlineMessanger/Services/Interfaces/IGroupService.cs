using OnlineMessanger.Models;

namespace OnlineMessanger.Services.Interfaces
{
    public interface IGroupService
    {
        Task<List<Group>> GetGroupsByUserId(string userId);
        Task<List<Group>> GetGroupsByIds(string[] groupIds);
        bool HasAccessToGroup(string userId, string groupId);
        Task CreateGroup(Group group);
        Task InviteToGroup(string email, string groupId);
        Task RemoveFromGroup(string email, string groupId, string requestorId);
        string GetMembersByGroupId(string groupId);
    }
}
