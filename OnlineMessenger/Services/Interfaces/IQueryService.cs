using Microsoft.Data.SqlClient;

namespace OnlineMessenger.Services.Interfaces
{
    public interface IQueryService : IAsyncDisposable, IDisposable
    {
        Task<SqlDataReader> Select(string fields, string source, string condition, string additionalOptions);
    }
}
