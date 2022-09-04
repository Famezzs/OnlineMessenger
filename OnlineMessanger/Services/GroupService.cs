using Microsoft.Data.SqlClient;

using OnlineMessanger.Helpers;
using OnlineMessanger.Helpers.Constants;
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

            var fields = "Id, Description, ImageUrl, Name, OwnerId";

            var source = "dbo.Groups";

            var arrayRepresentation = ArrayToSqlCompatible.Convert(groupIds);

            var condition = $"Id IN {arrayRepresentation}";

            using var queryService = new QueryService();

            var sqlReader = await queryService.Select(fields, source, condition);

            while (await sqlReader.ReadAsync())
            {
                var id = (string)sqlReader["Id"];

                var description = (string)sqlReader["Description"];

                var imageUrl = (string)sqlReader["ImageUrl"];

                var name = (string)sqlReader["Name"];

                var ownerId = (string)sqlReader["OwnerId"];

                groups.Add(new Group(id, name, description, imageUrl, ownerId));
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
            var user = context.Users.Where(user => user.Email == email).First();

            if (user == null)
            {
                throw new Exception(Constants._noSuchUserExistsError);
            }

            var group = context.Groups.Where(group => group.Id == groupId);

            if (!group.Any())
            {
                throw new Exception(Constants._noSuchGroupExistsError);
            }

            var groupMember = context.GroupMembers.Where(member => member.UserId == user.Id && member.GroupId == groupId);

            if (groupMember.Any())
            {
                throw new Exception(Constants._userIsAlreadyMemberError);
            }

            var invite = new GroupMember(user.Id, groupId);

            await context.GroupMembers.AddAsync(invite);

            await context.SaveChangesAsync();
        }

        public async Task RemoveFromGroup(string email, string groupId, string requestorId)
        {
            var userToRemove = context.Users.Where(user => user.Email == email).FirstOrDefault();

            if (userToRemove == null)
            {
                throw new Exception(Constants._noSuchUserExistsError);
            }

            if (userToRemove.Id == requestorId)
            {
                throw new Exception(Constants._cannotRemoveSelfError);
            }

            var group = context.Groups.Where(group => group.Id == groupId);

            if (!group.Any())
            {
                throw new Exception(Constants._noSuchGroupExistsError);
            }

            if (requestorId != group.First().OwnerId)
            {
                throw new Exception(Constants._notEnoughPermissionError);
            }

            var groupMember = context.GroupMembers.Where(member => member.UserId == userToRemove.Id && member.GroupId == groupId);

            if (!groupMember.Any())
            {
                throw new Exception(Constants._userIsNotMemberOfGroupError);
            }

            context.GroupMembers.Remove(groupMember.First());

            await context.SaveChangesAsync();
        }
        public string GetMembersByGroupId(string groupId)
        {
            if (String.IsNullOrWhiteSpace(groupId))
            {
                return string.Empty;
            }

            var groupInvitations = context.GroupMembers.Where(invite => invite.GroupId == groupId);

            if (!groupInvitations.Any())
            {
                return string.Empty;
            }

            var memberIds = new HashSet<string>();

            foreach (var invitation in groupInvitations)
            {
                memberIds.Add(invitation.UserId);
            }

            var members = context.Users.Where(user => memberIds.Contains(user.Id));

            if (!members.Any())
            {
                return string.Empty;
            }

            var result = string.Empty;

            foreach (var member in members)
            {
                result += member.Email + "<br>";
            }

            return result;
        }

        public GroupService(MessangerDataContext context)
        {
            this.context = context;
        }

        private MessangerDataContext context;
    }
}
