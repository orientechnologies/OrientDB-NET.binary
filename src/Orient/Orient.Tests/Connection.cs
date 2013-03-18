using Orient.Client;

namespace Orient.Tests
{
    public static class Connection
    {
        private static string _hostname = "127.0.0.1";
        private static int _port = 2424;
        private static string _username = "admin";
        private static string _password = "admin";
        private static int _poolSize = 10;

        private static string _rootUserName = "root";
        private static string _rootUserParssword = "D348621FCC37E9B0007147DBC2EF3DFAEA6402CAEC594BF8491C79D595CEEAF4";
        private static OServer _server;

        public static string GlobalTestDatabaseName { get; private set; }
        public static ODatabaseType GlobalTestDatabaseType { get; private set; }
        public static string GlobalTestDatabaseAlias { get; private set; }

        static Connection()
        {
            _server = new OServer(_hostname, _port, _rootUserName, _rootUserParssword);

            GlobalTestDatabaseName = "globalTestDatabaseForNetDriver001";
            GlobalTestDatabaseType = ODatabaseType.Graph;
            GlobalTestDatabaseAlias = "globalTestDatabaseForNetDriver001Alias";
        }

        public static void CreateDatabase(string name, ODatabaseType type, string alias)
        {
            DropDatabase(name);

            _server.CreateDatabase(name, type, OStorageType.Remote);

            OClient.CreateDatabasePool(
                _hostname,
                _port,
                name,
                type,
                _username,
                _password,
                _poolSize,
                alias
            );
        }

        public static ODatabase GetDatabase(string alias)
        {
            return new ODatabase(alias);
        }

        public static void DropDatabase(string name)
        {
            if (_server.DatabaseExist(name))
            {
                _server.DropDatabase(name);
            }
        }

        public static OServer GetServer()
        {
            return _server;
        }
    }
}
