using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orient.Client.API;
using Orient.Client.API.Query;
using Orient.Client.API.Query.Interfaces;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;
using Orient.Client.Protocol.Operations.Command;
using Orient.Client.Protocol.Serializers;
using OrientDB_Net.binary.Innov8tive.API;
using OrientDB_Net.binary.Innov8tive.Protocol;

namespace Orient.Client
{
    public class ODatabase : IDisposable
    {
        private bool _containsConnection;

        private ODocument _databaseProperties;

        public IDictionary<ORID, ODocument> ClientCache { get; private set; }

        public OCreate Create => new OCreate(GetConnection());
        public OSqlDelete Delete => new OSqlDelete(GetConnection());
        public OLoadRecord Load => new OLoadRecord(GetConnection());
        public ORecordMetadata Metadata => new ORecordMetadata(GetConnection());
        public OSqlSchema Schema => new OSqlSchema(GetConnection());

        private readonly ConnectionPool _connectionPool;

        public ODocument DatabaseProperties
        {
            get
            {
                if (_databaseProperties == null)
                {
                    _databaseProperties = retrieveDataBaseProperties().Result;
                }
                return _databaseProperties;
            }
        }

        public OTransaction Transaction => _connectionPool.GetConnection().ConnectionTransaction;

        internal Connection GetConnection()
        {
            var connection = _connectionPool.GetConnection();
            connection.Database = this;
            return connection;
        }

        public ODatabase(string hostName, int port, string databaseName, ODatabaseType type, string userName, string userPassword)
        {
            _connectionPool = new ConnectionPool(hostName, port, databaseName, type, userName, userPassword);
            ClientCache = new ConcurrentDictionary<ORID, ODocument>();
        }

        public ODatabase(ConnectionOptions options)
        {
            _connectionPool = new ConnectionPool(options.HostName, options.Port, options.DatabaseName, options.DatabaseType, options.UserName, options.Password);
            ClientCache = new ConcurrentDictionary<ORID, ODocument>();
        }

        public int ProtocolVersion => GetConnection().ProtocolVersion;

        private Task<ODocument> retrieveDataBaseProperties()
        {
            return Task.Factory.StartNew(() =>
            {
                var document = Load.ORID(new ORID(0, 0)).Run();

                byte[] rawByte = document.GetField<byte[]>("RawBytes");
                var str = Encoding.UTF8.GetString(rawByte, 0, rawByte.Length);
                var values = str.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var doc = new ODocument();
                doc.SetField("Version", values[0]);
                doc.SetField("LocaleLanguage", values[5]);
                doc.SetField("LocaleCountry", values[6]);
                doc.SetField("DateFormat", values[7]);
                doc.SetField("DateTimeFormat", values[8]);
                doc.SetField("Timezone", values[9]);
                doc.SetField("Charset", values[10]);
                return doc;
            });
        }

        public List<OCluster> GetClusters(bool reload = false)
        {
            if (!reload)
                return GetConnection().Document.GetField<List<OCluster>>("Clusters");

            GetConnection().Reload();
            return GetClusters();
        }

        public short GetClusterIdFor(string className)
        {
            return Schema.GetDefaultClusterForClass(className);
        }

        public string GetClusterNameFor(short clusterId)
        {
            OCluster oCluster = GetClusters().FirstOrDefault(x => x.Id == clusterId);
            if (oCluster == null)
            {
                GetConnection().Reload();
                oCluster = GetClusters().FirstOrDefault(x => x.Id == clusterId);
            }
            return oCluster.Name;
        }

        private string CorrectClassName(string className)
        {
            if (className == "OVertex")
                return "V";
            if (className == "OEdge")
                return "E";
            return className;
        }

        internal OCluster AddCluster(OCluster cluster)
        {
            var clusters = GetConnection().Document.GetField<List<OCluster>>("Clusters");
            if (!clusters.Contains(cluster))
                clusters.Add(cluster);
            return cluster;
        }

        internal void RemoveCluster(short clusterid)
        {
            var clusters = GetConnection().Document.GetField<List<OCluster>>("Clusters");
            var cluster = clusters.SingleOrDefault(c => c.Id == clusterid);

            if (cluster != null)
                clusters.Remove(cluster);
        }

        public OSqlSelect Select(params string[] projections)
        {
            return new OSqlSelect(GetConnection()).Select(projections);
        }

        #region Insert

        public IOInsert Insert()
        {
            return new OSqlInsert(GetConnection());
        }

        public IOInsert Insert<T>(T obj)
        {
            return new OSqlInsert(GetConnection())
                .Insert(obj);
        }

        #endregion

        #region Update

        public OSqlUpdate Update()
        {
            return new OSqlUpdate(GetConnection());
        }

        public OSqlUpdate Update(ORID orid)
        {
            return new OSqlUpdate(GetConnection())
                .Update(orid);
        }

        public OSqlUpdate Update<T>(T obj)
        {
            return new OSqlUpdate(GetConnection())
                .Update(obj);
        }

        #endregion

        #region Query

        public List<ODocument> Query(string sql)
        {
            return Query(sql, "*:0");
        }

        public List<T> Query<T>(string sql, string fetchPlan = "*:0") where T : class, new()
        {
            var docs = Query(sql, fetchPlan);
            List<T> convertedList = new List<T>();
            foreach (var d in docs)
                convertedList.Add(d.To<T>());
            return convertedList;
        }

        public List<ODocument> Query(string sql, string fetchPlan)
        {
            CommandPayloadQuery payload = new CommandPayloadQuery();
            payload.Text = sql;
            payload.NonTextLimit = -1;
            payload.FetchPlan = fetchPlan;

            Command operation = new Command(GetConnection().Database);
            operation.OperationMode = OperationMode.Asynchronous;
            operation.CommandPayload = payload;

            ODocument document = GetConnection().ExecuteOperation(operation);

            return document.GetField<List<ODocument>>("Content");
        }

        public PreparedQuery Query(PreparedQuery query)
        {
            query.SetConnection(GetConnection());
            return query;
        }

        #endregion

        public OCommandQuery SqlBatch(string batch)
        {
            CommandPayloadScript payload = new CommandPayloadScript();
            payload.Language = "sql";
            payload.Text = batch;

            return new OCommandQuery(GetConnection(), payload);
        }

        public OCommandResult Gremlin(string query)
        {
            CommandPayloadScript payload = new CommandPayloadScript();
            payload.Language = "gremlin";
            payload.Text = query;

            Command operation = new Command(GetConnection().Database);
            operation.OperationMode = OperationMode.Synchronous;
            operation.CommandPayload = payload;

            ODocument document = GetConnection().ExecuteOperation(operation);

            return new OCommandResult(document);
        }

        public OCommandQuery JavaScript(string query)
        {
            CommandPayloadScript payload = new CommandPayloadScript();
            payload.Language = "javascript";
            payload.Text = query;

            return new OCommandQuery(GetConnection(), payload);
        }

        public OCommandResult Command(string sql)
        {
            CommandPayloadCommand payload = new CommandPayloadCommand();
            payload.Text = sql;

            OCommandQuery query = new OCommandQuery(GetConnection(), payload);
            return query.Run();
        }

        public PreparedCommand Command(PreparedCommand command)
        {
            command.SetConnection(GetConnection());
            return command;
        }

        public long Size
        {
            get
            {
                var operation = new DBSize(GetConnection().Database);
                var document = GetConnection().ExecuteOperation(operation);
                return document.GetField<long>("size");
            }
        }

        public long CountRecords
        {
            get
            {
                var operation = new DBCountRecords(GetConnection().Database);
                var document = GetConnection().ExecuteOperation(operation);
                return document.GetField<long>("count");
            }
        }

        public void Close()
        {
            if (_containsConnection)
            {
                GetConnection().Database = null;

                GetConnection().Dispose();

                _containsConnection = false;
            }
        }

        public void Dispose()
        {
            Close();
        }

        public OClusterQuery Clusters(params string[] clusterNames)
        {
            return Clusters(clusterNames.Select(n => new OCluster { Name = n, Id = Schema.GetDefaultClusterForClass(n) }));
        }

        private OClusterQuery Clusters(IEnumerable<OCluster> clusters)
        {
            var query = new OClusterQuery(GetConnection());
            foreach (var id in clusters)
            {
                query.AddClusterId(id);
            }
            return query;
        }

        public OClusterQuery Clusters(params short[] clusterIds)
        {
            //return Clusters(clusterIds.Select(id => new OCluster { Name = GetClusterNameFor(id), Id = id }));
            return Clusters(clusterIds.Select(id => new OCluster { Id = id }));
        }

    }
}
