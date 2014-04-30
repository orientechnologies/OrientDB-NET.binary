using System.Collections.Generic;
using System.Linq;

namespace Orient.Client.Protocol
{
    internal class DatabasePool
    {
        private Queue<Connection> _connections;

        internal string Release { get; private set; }
        internal string Hostname { get; set; }
        internal int Port { get; set; }
        internal string DatabaseName { get; set; }
        internal ODatabaseType DatabaseType { get; set; }
        internal string UserName { get; set; }
        internal string UserPassword { get; set; }
        internal int PoolSize { get; private set; }
        internal string Alias { get; set; }
        internal int CurrentSize 
        { 
            get 
            {
                return _connections.Where(con => con.IsActive == true).Count(); 
            } 
        }

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
                Connection connection = new Connection(Hostname, Port, databaseName, databaseType, userName, userPassword, alias, true);

                _connections.Enqueue(connection);
            }

            //get release from last connection
            var lastConnection = _connections.LastOrDefault();
            if (lastConnection != null)
            {
                Release = lastConnection.Document.GetField<string>("OrientdbRelease");
            }
        }

        internal void DropConnections()
        {
            foreach (Connection connection in _connections)
            {
                connection.Dispose();
            }
        }

        internal Connection DequeueConnection()
        {
            return _connections.Dequeue();
        }

        internal void EnqueueConnection(Connection connection)
        {
            _connections.Enqueue(connection);
        }
    }
}
