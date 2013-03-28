using System.Collections.Generic;
using Orient.Client.Protocol;

namespace Orient.Client
{
    public static class OClient
    {
        private static object _syncRoot;
        private static List<DatabasePool> _databasePools;
        internal static string ClientID { get { return "null"; } }

        public static string DriverName { get { return "OrientDB-NET.binary"; } }
        public static string DriverVersion { get { return "Alpha 1"; } }
        public static short ProtocolVersion { get { return 13; } }
        public static int BufferLenght { get; set; }

        static OClient()
        {
            _syncRoot = new object();
            _databasePools = new List<DatabasePool>();
            BufferLenght = 1024;
        }

        public static void CreateDatabasePool(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword, int poolSize, string alias)
        {
            lock (_syncRoot)
            {
                DatabasePool databasePool = new DatabasePool(hostname, port, databaseName, databaseType, userName, userPassword, poolSize, alias);

                _databasePools.Add(databasePool);
            }
        }

        public static void DropDatabasePool(string alias)
        {
            lock (_syncRoot)
            {
                if (_databasePools.Exists(db => db.Alias == alias))
                {
                    DatabasePool pool = _databasePools.Find(db => db.Alias == alias);

                    pool.DropConnections();

                    _databasePools.Remove(pool);
                }
            }
        }

        internal static Connection ReleaseConnection(string alias)
        {
            lock (_syncRoot)
            {
                Connection connection = null;

                if (_databasePools.Exists(db => db.Alias == alias))
                {
                    DatabasePool pool = _databasePools.Find(db => db.Alias == alias);

                    // deque free database connection if the pool has one
                    if (pool.CurrentSize > 0)
                    {
                        connection = pool.DequeueConnection();
                    }
                    // if the pool is empty - create new dedicated database connection
                    else if (pool.CurrentSize == 0)
                    {
                        connection = new Connection(pool.Hostname, pool.Port, pool.DatabaseName, pool.DatabaseType, pool.UserName, pool.UserPassword, alias, false);
                    }
                }

                return connection;
            }
        }

        internal static void ReturnConnection(Connection connection)
        {
            lock (_syncRoot)
            {
                if (_databasePools.Exists(db => db.Alias == connection.Alias) && connection.IsReusable)
                {
                    DatabasePool pool = _databasePools.Find(q => q.Alias == connection.Alias);

                    pool.EnqueueConnection(connection);
                }
                else
                {
                    connection.Dispose();
                }
            }
        }
    }
}
