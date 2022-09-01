using Microsoft.Data.SqlClient;

using OnlineMessanger.Helpers;
using OnlineMessanger.Services.Interfaces;

namespace OnlineMessanger.Services
{
    public class QueryService : IQueryService
    {
        public async Task<SqlDataReader> Select(string fields, string source, string condition, string additionalOptions = "")
        {
            sqlConnection = new SqlConnection(ConnectionStrings.GetSqlConnectionString());

            await sqlConnection.OpenAsync();

            sqlCommand = new SqlCommand($"SELECT {fields} FROM {source} WHERE {condition} {additionalOptions}", sqlConnection);

            sqlReader = await sqlCommand.ExecuteReaderAsync();

            return sqlReader;
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            GC.SuppressFinalize(this);

            if (sqlConnection != null)
            {
                await sqlConnection.DisposeAsync();
            }

            if (sqlCommand != null)
            {
                await sqlCommand.DisposeAsync();
            }

            if (sqlReader != null)
            {
                await sqlReader.DisposeAsync();
            }
        }

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);

            if (sqlConnection != null)
            {
                sqlConnection.Dispose();
            }

            if (sqlCommand != null)
            {
                sqlCommand.Dispose();
            }

            if (sqlReader != null)
            {
                sqlReader.Dispose();
            }
        }

        private SqlConnection? sqlConnection;

        private SqlCommand? sqlCommand;

        private SqlDataReader? sqlReader;
    }
}
