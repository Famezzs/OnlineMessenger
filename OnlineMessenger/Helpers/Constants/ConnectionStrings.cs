namespace OnlineMessenger.Helpers.Constants
{
    public class ConnectionStrings
    {
        public static ConnectionStrings GetInstance(string sqlConnectionString)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ConnectionStrings(sqlConnectionString);
                    }
                }
            }

            return _instance;
        }

        public static string GetSqlConnectionString()
        {
            if (_instance != null)
            {
                return _instance.sqlConnectionString;
            }
            else
            {
                return string.Empty;
            }
        }

        private ConnectionStrings(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        private readonly string sqlConnectionString;

        private static ConnectionStrings? _instance;

        private static readonly object _lock = new object();
    }
}
