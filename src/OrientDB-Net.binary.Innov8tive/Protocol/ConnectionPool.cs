using System.Collections.Concurrent;
using System.Threading;
using Orient.Client;
using Orient.Client.Protocol;

namespace OrientDB_Net.binary.Innov8tive.Protocol
{
    internal class ConnectionPool
    {
        private readonly string _hostName;
        private readonly int _port;
        private readonly string _databaseName;
        private readonly ODatabaseType _type;
        private readonly string _userName;
        private readonly string _userPassword;

        public ConnectionPool(string hostName, int port, string databaseName, ODatabaseType type, string userName, string userPassword)
        {
            _hostName = hostName;
            _port = port;
            _databaseName = databaseName;
            _type = type;
            _userName = userName;
            _userPassword = userPassword;
        }

        private readonly ConcurrentDictionary<int, Connection> _connectionPool = new ConcurrentDictionary<int, Connection>();

        public Connection GetConnection()
        {
            if (_connectionPool.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                return _connectionPool[Thread.CurrentThread.ManagedThreadId];

            var connection = new Connection(_hostName, _port, _databaseName, _type, _userName, _userPassword);
            _connectionPool.AddOrUpdate(Thread.CurrentThread.ManagedThreadId, i => connection,
                (i, conn) => _connectionPool[i] = conn);

            return connection;
        }
    }
}
