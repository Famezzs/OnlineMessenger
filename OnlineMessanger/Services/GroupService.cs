using Microsoft.Data.SqlClient;

using OnlineMessanger.Helpers;
using OnlineMessanger.Models;
using OnlineMessanger.Services.Interfaces;

namespace OnlineMessanger.Services
{
    public class GroupService : IGroupService
    {
        public async Task<List<Group>> GetGroupsByUserId(string userId)
        {
            var groupIds = new List<string>();

            var fields = "GroupId";

            var source = "dbo.GroupMembers";

            var condition = $"UserId='{userId}'";

            using var queryService = new QueryService();

            var sqlReader = await queryService.Select(fields, source, condition);

            while (await sqlReader.ReadAsync())
            {
                var groupId = (string)sqlReader["GroupId"];

                groupIds.Add(groupId);
            }

            return await GetGroupsByIds(groupIds.ToArray());
        }

        public async Task<List<Group>> GetGroupsByIds(string[] groupIds)
        {
            var groups = new List<Group>();

            var fields = "Id, IsPublic, Description, ImageUrl, Name, OwnerId";

            var source = "dbo.Groups";

            var arrayRepresentation = ArrayToSqlCompatible.Convert(groupIds);

            var condition = $"Id IN {arrayRepresentation}";

            using var queryService = new QueryService();

            var sqlReader = await queryService.Select(fields, source, condition);

            while (await sqlReader.ReadAsync())
            {
                var id = (string)sqlReader["Id"];

                var isPublic = (bool)sqlReader["IsPublic"];

                var description = (string)sqlReader["Description"];

                var imageUrl = (string)sqlReader["ImageUrl"];

                var name = (string)sqlReader["Name"];

                var ownerId = (string)sqlReader["OwnerId"];

                groups.Add(new Group(id, name, description, imageUrl, ownerId, isPublic));
            }

            return groups;
        }

        public bool HasAccessToGroup(string userId, string groupId)
        {
            var member = context!.GroupMembers!.Where(entry => entry.UserId == userId && entry.GroupId == groupId);

            if (member == null || 
                !member.Any())
            {
                return false;
            }

            return true;
        }

        public async Task CreateGroup(Group group)
        {
            var groupMember = new GroupMember(group.OwnerId, group.Id);

            await context.AddAsync(group);

            await context.AddAsync(groupMember);

            await context.SaveChangesAsync();
        }

        public async Task InviteToGroup(string email, string groupId)
        {
            if (String.IsNullOrWhiteSpace(email) || 
                String.IsNullOrWhiteSpace(groupId))
            {
                return;
            }

            var user = context.Users.Where(user => user.Email == email).First();

            if (user == null)
            {
                return;
            }

            var groupMember = context.GroupMembers.Where(member => member.UserId == user.Id && member.GroupId == groupId);

            if (groupMember.Any())
            {
                return;
            }

            var invite = new GroupMember(user.Id, groupId);

            await context.GroupMembers.AddAsync(invite);

            await context.SaveChangesAsync();
        }

        public GroupService(MessangerDataContext context)
        {
            this.context = context;
        }

        private MessangerDataContext context;
    }
}
