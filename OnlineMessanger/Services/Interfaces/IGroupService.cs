using OnlineMessanger.Models;

namespace OnlineMessanger.Services.Interfaces
{
    public interface IGroupService
    {
        Task<List<Group>> GetGroupsByUserId(string userId);
    }
}
