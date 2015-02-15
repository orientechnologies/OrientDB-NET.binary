using Orient.Client;

namespace Orient.Tests
{
    public static class TestConnection
    {
        private static string _hostname = "127.0.0.1";
        private static int _port = 2424;
        private static string _username = "admin";
        private static string _password = "admin";

        private static string _rootUserName = "root";
        private static string _rootUserParssword = "root";
        private static OServer _server;

        public static int GlobalTestDatabasePoolSize { get { return 3; } }
        public static string GlobalTestDatabaseName { get; private set; }
        public static ODatabaseType GlobalTestDatabaseType { get; private set; }
        public static string GlobalTestDatabaseAlias { get; private set; }
        public static OStorageType GlobalTestDatabaseStorageType { get; private set; }
        static TestConnection()
        {
            _server = new OServer(_hostname, _port, _rootUserName, _rootUserParssword);
            //testing
            GlobalTestDatabaseName = "globalTestDatabaseForNetDriver001";
            GlobalTestDatabaseType = ODatabaseType.Graph;
            GlobalTestDatabaseAlias = "globalTestDatabaseForNetDriver001Alias";
            GlobalTestDatabaseStorageType = OStorageType.PLocal;
        }

        public static void CreateTestDatabase()
        {
            DropTestDatabase();

            _server.CreateDatabase(GlobalTestDatabaseName, GlobalTestDatabaseType, GlobalTestDatabaseStorageType);
        }

        public static void DropTestDatabase()
        {
            var retry = 2;
            do
            {
                try
                {
                    if (_server.DatabaseExist(GlobalTestDatabaseName, GlobalTestDatabaseStorageType))
                    {
                        _server.DropDatabase(GlobalTestDatabaseName, GlobalTestDatabaseStorageType);
                    }
                    break;
                }
                catch
                {
                    _server.Dispose();
                    _server = new OServer(_hostname, _port, _rootUserName, _rootUserParssword);
                }
            }
            while (--retry > 0);
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
