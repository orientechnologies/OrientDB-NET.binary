using Orient.Client;

namespace Orient.Tests
{
    public static class Connection
    {
        private static string _hostname = "127.0.0.1";
        private static int _port = 2424;
        private static string _databaseName = "";
        private static ODatabaseType _databaseType = ODatabaseType.Graph;
        private static string _username = "admin";
        private static string _password = "admin";
        private static int _poolSize = 10;

        public static string DatabaseAlias = "tinkerpop";

        public Connection()
        {
            OClient.CreateDatabasePool(
                _hostname,
                _port,
                _databaseName,
                _databaseType,
                _username,
                _password,
                _poolSize,
                DatabaseAlias
            );
        }

        public static ODatabase GetDatabase(string alias)
        {
            return new ODatabase(alias);
        }
    }
}
