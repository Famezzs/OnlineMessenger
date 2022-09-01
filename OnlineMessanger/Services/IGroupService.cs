using OnlineMessanger.Models;

namespace OnlineMessanger.Services
{
    public interface IGroupService
    {
        Task<List<Group>> GetGroupsByUserId(string userId);
    }
}
