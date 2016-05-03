using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Orient.Client.Protocol
{
    internal class DatabasePool
    {
        private int _currentSize;
        private ConcurrentBag<Connection> _connections;

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
                return _currentSize;
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

            _connections = new ConcurrentBag<Connection>();

            EstablishConnections();
        }

        internal void EstablishConnections()
        {
            for (int i = 0; i < this.PoolSize; i++)
            {
                var connection = new Connection(Hostname, Port, DatabaseName, DatabaseType, UserName, UserPassword, Alias, true);

                _connections.Add(connection);
                Interlocked.Increment(ref _currentSize);
            }

            //get release from last connection
            var lastConnection = _connections.LastOrDefault();
            if (lastConnection != null)
            {
                Release = lastConnection.Document.GetField<string>("OrientdbRelease");
            }
        }

        internal void DropAndEstablishAllConnections()
        {
            DropConnections();
            EstablishConnections();
        }

        internal void DropConnections()
        {            
            Connection connectionToDrop = null;
            while (_connections.TryTake(out connectionToDrop))
            {
                Interlocked.Decrement(ref _currentSize);
                if (connectionToDrop.IsActive)
                {
                    connectionToDrop.Dispose();
                }
                else
                {
                    connectionToDrop.Destroy();
                }
            }
        }

        internal Connection DequeueConnection()
        {
            Connection connection = null;
            while (_connections.TryTake(out connection))
            {
                Interlocked.Decrement(ref _currentSize);
                if (connection.IsActive)
                {
                    return connection;
                }
                else
                {                    
                    connection.Destroy();
                }
            }

            return new Connection(Hostname, Port, DatabaseName, DatabaseType, UserName, UserPassword, Alias, true);
        }

        internal bool TryEnqueueConnection(Connection connection)
        {
            if (connection.IsActive && this.CurrentSize < this.PoolSize)
            {
                _connections.Add(connection);
                Interlocked.Increment(ref _currentSize);
                return true;
            }

            return false;
        }
    }
}
