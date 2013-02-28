using System.Collections.Generic;

namespace Orient.Client.Protocol
{
    internal class DatabasePool
    {
        private Queue<Connection> _connections;

        internal string Hostname { get; set; }
        internal int Port { get; set; }
        internal string DatabaseName { get; set; }
        internal ODatabaseType DatabaseType { get; set; }
        internal string UserName { get; set; }
        internal string UserPassword { get; set; }
        internal int PoolSize { get; private set; }
        internal string Alias { get; set; }
        internal int CurrentPoolSize { get { return _connections.Count; } }

        internal DatabasePool(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword, int poolSize, string alias)
        {
            Hostname = hostname;
            Port = port;
            DatabaseName = databaseName;
            DatabaseType = databaseType;
            UserName = userName;
            UserPassword = userPassword;
            PoolSize = poolSize;
            Alias = alias;

            _connections = new Queue<Connection>();

            for (int i = 0; i < poolSize; i++)
            {
                Connection connection = new Connection(Hostname, Port, true);

                _connections.Enqueue(connection);
            }
        }
    }
}
