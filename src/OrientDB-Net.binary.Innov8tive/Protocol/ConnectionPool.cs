using System.Collections.Concurrent;
using System.Threading;
using Orient.Client;
using Orient.Client.Protocol;
using System.Security.Cryptography.X509Certificates;

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
        private readonly string _poolAlias;
        private readonly X509Certificate2Collection _sslCerts;

        public ConnectionPool(string hostName, int port, string databaseName, ODatabaseType type, string userName, string userPassword)
        {
            _hostName = hostName;
            _port = port;
            _databaseName = databaseName;
            _type = type;
            _userName = userName;
            _userPassword = userPassword;
            _poolAlias = "Default";
            _sslCerts = null;
        }

        public ConnectionPool(string hostName, int port, string databaseName, ODatabaseType type, string userName, string userPassword, string poolAlias)
        {
            _hostName = hostName;
            _port = port;
            _databaseName = databaseName;
            _type = type;
            _userName = userName;
            _userPassword = userPassword;
            _poolAlias = poolAlias;
            _sslCerts = null;
        }
        public ConnectionPool(string hostName, int port, string databaseName, ODatabaseType type, string userName, string userPassword, X509Certificate2Collection sslCerts)
        {
            _hostName = hostName;
            _port = port;
            _databaseName = databaseName;
            _type = type;
            _userName = userName;
            _userPassword = userPassword;
            _poolAlias = "sslDefault";
            _sslCerts = sslCerts;
        }
        private readonly ConcurrentDictionary<string, Connection> _connectionPool = new ConcurrentDictionary<string, Connection>();

        public Connection GetConnection()
        {
            if (_connectionPool.ContainsKey(_poolAlias))
                return _connectionPool[_poolAlias];

            var connection = new Connection(_hostName, _port, _databaseName, _type, _userName, _userPassword, _sslCerts);
            _connectionPool.AddOrUpdate(_poolAlias, i => connection,
                (i, conn) => _connectionPool[i] = conn);

            return connection;
        }
    }
}
