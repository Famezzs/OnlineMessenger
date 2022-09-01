using Microsoft.Data.SqlClient;
using OnlineMessanger.Helpers;
using OnlineMessanger.Models;

namespace OnlineMessanger.Services
{
    public class GroupService : IGroupService
    {
        public async Task<List<Group>> GetGroupsByUserId(string userId)
        {
            var groupId = string.Empty;

            var groups = new List<Group>();

            using (var sqlConnection = new SqlConnection(ConnectionStrings.GetSqlConnectionString()))
            {
                await sqlConnection.OpenAsync();

                using var sqlCommand = new SqlCommand($"SELECT GroupId FROM dbo.GroupMembers WHERE UserId='{userId}'", sqlConnection);

                using var reader = await sqlCommand.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    groupId = (string)reader["GroupId"];

                    var group = await context.Groups!.FindAsync(groupId);

                    if (group != null)
                    {
                        groups.Add(group);
                    }
                }
            }

            return groups;
        }

        public GroupService(MessangerDataContext context)
        {
            this.context = context;
        }

        private MessangerDataContext context;
    }
}
