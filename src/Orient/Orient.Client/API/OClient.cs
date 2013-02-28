using System.Collections.Generic;
using Orient.Client.Protocol;

namespace Orient.Client
{
    public static class OClient
    {
        private static object _syncRoot;
        private static List<DatabasePool> _databasePools;

        public static string DriverName { get { return "OrientDB-NET.binary"; } }
        public static string DriverVersion { get { return "Alpha 1"; } }

        static OClient()
        {
            _syncRoot = new object();
            _databasePools = new List<DatabasePool>();
        }

        public static void CreateDatabasePool(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword, int poolSize, string alias)
        {
            lock (_syncRoot)
            {
                DatabasePool databasePool = new DatabasePool(hostname, port, databaseName, databaseType, userName, userPassword, poolSize, alias);

                _databasePools.Add(databasePool);
            }
        }
    }
}
