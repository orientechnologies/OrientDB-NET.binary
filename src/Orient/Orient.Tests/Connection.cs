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

        private static string _rootUserName = "root";
        private static string _rootUserParssword = "D348621FCC37E9B0007147DBC2EF3DFAEA6402CAEC594BF8491C79D595CEEAF4";
        private static OServer _server;

        public static string DatabaseAlias = "tinkerpop";

        private Connection()
        {
            /*OClient.CreateDatabasePool(
                _hostname,
                _port,
                _databaseName,
                _databaseType,
                _username,
                _password,
                _poolSize,
                DatabaseAlias
            );*/

            _server = new OServer(_hostname, _port, _rootUserName, _rootUserParssword);
        }

        public static ODatabase GetDatabase(string alias)
        {
            return new ODatabase(alias);
        }

        public static OServer GetServer()
        {
            return _server;
        }

        public void Dispose()
        {
            _server.Dispose();
        }
    }
}
