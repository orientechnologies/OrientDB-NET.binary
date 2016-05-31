using System.Collections.Generic;
using System.Linq;
using Orient.Client.API.Types;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client
{
    public static class OClient
    {
        private static object _syncRoot;
        private static List<DatabasePool> _databasePools;
        internal static string ClientID { get; set; }
        private static short _protocolVersion = 21;
        public static string DriverName { get { return "OrientDB-NET.binary"; } }
        public static string DriverVersion { get { return "0.2.1"; } }

        public static short ProtocolVersion
        {
            get { return _protocolVersion; }
            set
            {
                if (value != _protocolVersion)
                    _protocolVersion = value;
            }
        }

        public static int BufferLenght { get; set; }
        public static ORecordFormat Serializer { get; set; }
        public static bool UseTokenBasedSession { get; set; }

        public static string SerializationImpl { get { return Serializer.ToString(); } }

        static OClient()
        {
            _syncRoot = new object();
            _databasePools = new List<DatabasePool>();
            BufferLenght = 1024;
            Serializer = ORecordFormat.ORecordDocument2csv;
            ClientID = "null";
            /* 
              If you enable token based session make shure enable it in server config
              <!-- USE SESSION TOKEN, TO TURN ON SET THE 'ENABLED' PARAMETER TO 'true' -->
              <handler class="com.orientechnologies.orient.server.token.OrientTokenHandler">
                  <parameters>
                      <parameter name="enabled" value="true"/>
                      <!-- PRIVATE KEY -->
                      <parameter name="oAuth2Key" value="GVsbG8gd29ybGQgdGhpcyBpcyBteSBzZWNyZXQgc2VjcmV0"/>
                      <!-- SESSION LENGTH IN MINUTES, DEFAULT=1 HOUR -->
                      <parameter name="sessionLength" value="60"/>
                      <!-- ENCRYPTION ALGORITHM, DEFAULT=HmacSHA256 -->
                      <parameter name="encryptionAlgorithm" value="HmacSHA256"/>
                  </parameters>            
               </handler>
             */
            UseTokenBasedSession = false;
        }

        public static string CreateDatabasePool(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword, int poolSize, string alias, string clientID = "null")
        {
            OClient.ClientID = clientID;

            lock (_syncRoot)
            {
                DatabasePool databasePool = _databasePools.FirstOrDefault(db => db.Alias == alias);
                if (databasePool == null)
                {
                    databasePool = new DatabasePool(hostname, port, databaseName, databaseType, userName, userPassword, poolSize, alias);

                    _databasePools.Add(databasePool); 
                }

                return databasePool.Release;
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

        public static int DatabasePoolCurrentSize(string alias)
        {
            if (_databasePools.Exists(db => db.Alias == alias))
            {
                DatabasePool pool = _databasePools.Find(db => db.Alias == alias);

                return pool.CurrentSize;
            }

            return -1;
        }

        /// <summary>
        /// Checks to see if a database pool has already been created with the given alias.
        /// </summary>
        /// <param name="alias">The name assigned to the database pool.</param>
        /// <returns>True if a pool exists.</returns>
        public static bool PoolExists(string alias) => _databasePools.Exists(db => db.Alias == alias);

        internal static Connection ReleaseConnection(string alias)
        {
            lock (_syncRoot)
            {
                Connection connection = null;

                if (_databasePools.Exists(db => db.Alias == alias))
                {
                    DatabasePool pool = _databasePools.Find(db => db.Alias == alias);

                    if (pool != null)
                    {
                        // deque free database connection if the pool has one
                        if (pool.CurrentSize > 0)
                        {
                            connection = pool.DequeueConnection();
                        }
                        // if the pool is empty - create new dedicated database connection
                        else if (pool.CurrentSize == 0)
                        {
                            connection = new Connection(pool.Hostname, pool.Port, pool.DatabaseName, pool.DatabaseType, pool.UserName, pool.UserPassword, alias, true);
                        }
                    }
                }

                return connection;
            }
        }

        internal static void ReturnConnection(Connection connection)
        {
            lock (_syncRoot)
            {
                DatabasePool pool = _databasePools.Find(q => q.Alias == connection.Alias);

                // enqueue the connection back if it's active, reusable and the pool size is not full
                // otherwise dispose it
                if ((pool != null) &&
                    (pool.CurrentSize < pool.PoolSize) &&
                    connection.IsActive &&
                    connection.IsReusable)
                {
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
