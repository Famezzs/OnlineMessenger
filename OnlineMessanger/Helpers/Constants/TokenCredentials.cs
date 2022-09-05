namespace OnlineMessanger.Helpers.Constants
{
    public class TokenCredentials
    {
        public static TokenCredentials GetInstance(string key, string issuer, string audience)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new TokenCredentials(key, issuer, audience);
                    }
                }
            }

            return _instance;
        }

        public static string GetSecurityKey()
        {
            if (_instance != null)
            {
                return _instance.key;
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetIssuer()
        {
            if (_instance != null)
            {
                return _instance.issuer;
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetAudience()
        {
            if (_instance != null)
            {
                return _instance.audience;
            }
            else
            {
                return string.Empty;
            }
        }

        private TokenCredentials(string key, string issuer, string audience)
        {
            this.key = key;
            this.issuer = issuer;
            this.audience = audience;
        }

        private readonly string key;

        private readonly string issuer;

        private readonly string audience;

        private static TokenCredentials? _instance;

        private static readonly object _lock = new object();
    }
}
