using Orient.Client;
using OrientDB_Net.binary.Innov8tive.API;

namespace Orient.Nunit.Test
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public static class TestConnection
    {
        private static string _hostname = "localhost";
        private static int _port = 2424;
        private static string _username = "root";
        private static string _password = "root";

        private static string _rootUserName = "root";
        private static string _rootUserParssword = "root";
        private static OServer _server;

        public static int GlobalTestDatabasePoolSize { get { return 3; } }
        public static string GlobalTestDatabaseName { get; private set; }
        public static ODatabaseType GlobalTestDatabaseType { get; private set; }
        public static string GlobalTestDatabaseAlias { get; private set; }

        public static ConnectionOptions ConnectionOptions
        {
            get
            {
                return new ConnectionOptions()
                {
                    DatabaseType = GlobalTestDatabaseType,
                    Port = _port,
                    Password = _password,
                    HostName = _hostname,
                    DatabaseName = GlobalTestDatabaseName,
                    UserName = _username
                };
            }
        }

        static TestConnection()
        {
            _server = new OServer(_hostname, _port, _rootUserName, _rootUserParssword);

            //GlobalTestDatabaseName = "globalTestDatabaseForNetDriver001";
            GlobalTestDatabaseName = "ModelTest";
            GlobalTestDatabaseType = ODatabaseType.Graph;
            GlobalTestDatabaseAlias = "globalTestDatabaseForNetDriver001Alias";
        }

        public static void CreateTestDatabase()
        {
            DropTestDatabase();

            _server.CreateDatabase(GlobalTestDatabaseName, GlobalTestDatabaseType, OStorageType.PLocal);
        }

        public static void DropTestDatabase()
        {
            if (_server.DatabaseExist(GlobalTestDatabaseName, OStorageType.PLocal))
            {
                _server.DropDatabase(GlobalTestDatabaseName, OStorageType.PLocal);
            }
        }

        public static OServer GetServer()
        {
            return _server;
        }
    }
}
