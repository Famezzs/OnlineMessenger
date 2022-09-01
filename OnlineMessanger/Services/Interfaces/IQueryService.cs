using Microsoft.Data.SqlClient;

namespace OnlineMessanger.Services.Interfaces
{
    public interface IQueryService : IAsyncDisposable, IDisposable
    {
        Task<SqlDataReader> Select(string fields, string source, string condition);
    }
}
