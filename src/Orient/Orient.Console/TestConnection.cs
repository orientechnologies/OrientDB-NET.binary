using Orient.Client;

namespace Orient.Console
{
    public static class TestConnection
    {
        private static string _hostname = "127.0.0.1";
        private static int _port = 2424;
        private static string _username = "admin";
        private static string _password = "admin";

        private static string _rootUserName = "root";
        private static string _rootUserParssword = "6ECADDC4AB2BE7447A3144588321247613B43D6EF04C8B3F301ABA48CE33CB89";
        private static OServer _server;

        public static int GlobalTestDatabasePoolSize { get { return 3; } }
        public static string GlobalTestDatabaseName { get; private set; }
        public static ODatabaseType GlobalTestDatabaseType { get; private set; }
        public static string GlobalTestDatabaseAlias { get; private set; }

        static TestConnection()
        {
            _server = new OServer(_hostname, _port, _rootUserName, _rootUserParssword);

            GlobalTestDatabaseName = "globalTestDatabaseForNetDriver001";
            GlobalTestDatabaseType = ODatabaseType.Graph;
            GlobalTestDatabaseAlias = "globalTestDatabaseForNetDriver001Alias";
        }

        public static void CreateTestDatabase()
        {
            DropTestDatabase();

            _server.CreateDatabase(GlobalTestDatabaseName, GlobalTestDatabaseType, OStorageType.Local);
        }

        public static void DropTestDatabase()
        {
            if (_server.DatabaseExist(GlobalTestDatabaseName))
            {
                _server.DropDatabase(GlobalTestDatabaseName);
            }
        }

        public static void CreateTestPool()
        {
            OClient.CreateDatabasePool(
                _hostname,
                _port,
                GlobalTestDatabaseName,
                GlobalTestDatabaseType,
                _username,
                _password,
                GlobalTestDatabasePoolSize,
                GlobalTestDatabaseAlias
            );
        }

        public static void DropTestPool()
        {
            OClient.DropDatabasePool(GlobalTestDatabaseAlias);
        }

        public static OServer GetServer()
        {
            return _server;
        }
    }
}
