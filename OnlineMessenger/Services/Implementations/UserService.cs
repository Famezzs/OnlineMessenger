using Microsoft.Data.SqlClient;
using OnlineMessenger.Helpers.Constants;
using OnlineMessenger.Models;
using OnlineMessenger.Services.Interfaces;

namespace OnlineMessenger.Services.Implementations
{
    public class UserService : IUserService
    {
        public async Task<User?> FindUserByEmail(string email)
        {
            User? user = null;

            using (var sqlConnection = new SqlConnection(ConnectionStrings.GetSqlConnectionString()))
            {
                await sqlConnection.OpenAsync();

                using var sqlCommand = new SqlCommand($"SELECT Id FROM dbo.AspNetUsers WHERE Email='{email}'", sqlConnection);

                using var reader = await sqlCommand.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var userId = (string)reader["Id"];

                    if (userId != null)
                    {
                        user = new User(email, email);

                        user.Id = userId;

                        break;
                    }
                }
            }

            return user;
        }
    }
}
