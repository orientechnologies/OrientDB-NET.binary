using System.Collections.Generic;
using Orient.Client.API.Types;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client
{
    public static class OClient
    {
        private static Dictionary<string, DatabasePool> _databasePools;
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
            _databasePools = new Dictionary<string, DatabasePool>();
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
            var databasePool = new DatabasePool(hostname, port, databaseName, databaseType, userName, userPassword, poolSize, alias);
            _databasePools.Add(alias, databasePool);
            return databasePool.Release;
        }

        public static void DropDatabasePool(string alias)
        {
            DatabasePool poolToDrop = null;
            if (_databasePools.TryGetValue(alias, out poolToDrop))
            {
                _databasePools.Remove(alias);
                poolToDrop.DropConnections();
            }
        }

        public static int DatabasePoolCurrentSize(string alias)
        {
            DatabasePool pool = null;
            if (_databasePools.TryGetValue(alias, out pool))
            {
                return pool.CurrentSize;
            }

            return -1;
        }

        internal static void DropAndEstablishConnectionsInAllPools()
        {
            foreach (var pool in _databasePools.Values)
            {
                pool.DropAndEstablishAllConnections();
            }
        }

        internal static Connection ReleaseConnection(string alias)
        {
            return _databasePools[alias].DequeueConnection();            
        }

        internal static void ReturnConnection(Connection connection)
        {
            DatabasePool pool;
            if (!_databasePools.TryGetValue(connection.Alias, out pool) || !pool.TryEnqueueConnection(connection))
            {
                // dispose the connection if the pool does not exists, or we are unable to enqueue it.
                connection.Dispose();
            }
        }

    }
}
