using System;
using System.Collections.Generic;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

namespace Orient.Client
{
    public class ODatabase : IDisposable
    {
        private bool _containsConnection;
        private Connection _connection;

        public IDictionary<ORID, ODocument> ClientCache { get; private set; }

        public OSqlCreate Create { get { return new OSqlCreate(_connection); } }
        public OSqlDelete Delete { get { return new OSqlDelete(_connection); } }

        public ODatabase(string alias)
        {
            _connection = OClient.ReleaseConnection(alias);
            _connection.Database = this;
            _containsConnection = true;
            ClientCache = new Dictionary<ORID, ODocument>();
        }

        public List<OCluster> GetClusters()
        {
            return _connection.Document.GetField<List<OCluster>>("Clusters");
        }

        public OSqlSelect Select(params string[] projections)
        {
            return new OSqlSelect(_connection).Select(projections);
        }

        #region Insert

        public OSqlInsert Insert()
        {
            return new OSqlInsert(_connection);
        }

        public OSqlInsert Insert<T>(T obj)
        {
            return new OSqlInsert(_connection)
                .Insert(obj);
        }

        #endregion

        #region Update

        public OSqlUpdate Update()
        {
            return new OSqlUpdate(_connection);
        }

        public OSqlUpdate Update(ORID orid)
        {
            return new OSqlUpdate(_connection)
                .Update(orid);
        }

        public OSqlUpdate Update<T>(T obj)
        {
            return new OSqlUpdate(_connection)
                .Update(obj);
        }

        #endregion

        #region Query

        public List<ODocument> Query(string sql)
        {
            return Query(sql, "*:0");
        }

        public List<ODocument> Query(string sql, string fetchPlan)
        {
            CommandPayload payload = new CommandPayload();
            payload.Type = CommandPayloadType.Sql;
            payload.Text = sql;
            payload.NonTextLimit = -1;
            payload.FetchPlan = fetchPlan;
            payload.SerializedParams = new byte[] { 0 };

            Command operation = new Command();
            operation.OperationMode = OperationMode.Asynchronous;
            operation.ClassType = CommandClassType.Idempotent;
            operation.CommandPayload = payload;

            ODocument document = _connection.ExecuteOperation<Command>(operation);

            return document.GetField<List<ODocument>>("Content");
        }

        #endregion

        public List<ODocument> Gremlin(string query)
        {
            CommandPayload payload = new CommandPayload();
            payload.Language = "gremlin";
            payload.Type = CommandPayloadType.Sql;
            payload.Text = query;
            payload.NonTextLimit = -1;
            payload.FetchPlan = "";
            payload.SerializedParams = new byte[] { 0 };

            Command operation = new Command();
            operation.OperationMode = OperationMode.Asynchronous;
            operation.ClassType = CommandClassType.NonIdempotent;
            operation.CommandPayload = payload;

            ODocument document = _connection.ExecuteOperation<Command>(operation);

            return document.GetField<List<ODocument>>("Content");
        }

        public OCommandResult Command(string sql)
        {
            CommandPayload payload = new CommandPayload();
            payload.Type = CommandPayloadType.Sql;
            payload.Text = sql;
            payload.NonTextLimit = -1;
            payload.FetchPlan = "";
            payload.SerializedParams = new byte[] { 0 };

            Command operation = new Command();
            operation.OperationMode = OperationMode.Synchronous;
            operation.ClassType = CommandClassType.NonIdempotent;
            operation.CommandPayload = payload;

            ODocument document = _connection.ExecuteOperation<Command>(operation);

            return new OCommandResult(document);
        }

        public void Close()
        {
            if (_containsConnection)
            {
                _connection.Database = null;

                if (_connection.IsReusable)
                {
                    OClient.ReturnConnection(_connection);
                }
                else
                {
                    _connection.Dispose();
                }
                
                _containsConnection = false;
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
