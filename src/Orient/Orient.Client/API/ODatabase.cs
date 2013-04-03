using System;
using System.Collections.Generic;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

namespace Orient.Client
{
    public class ODatabase : IDisposable
    {
        private Connection _connection;

        public OSqlCreate Create { get { return new OSqlCreate(_connection); } }

        public ODatabase(string alias)
        {
            _connection = OClient.ReleaseConnection(alias);
        }

        public List<OCluster> GetClusters()
        {
            return _connection.Document.GetField<List<OCluster>>("Clusters");
        }

        #region Select

        public OSqlSelect Select(string projection)
        {
            return new OSqlSelect(_connection).Select(projection);
        }

        public OSqlSelect Select(params string[] projections)
        {
            return new OSqlSelect(_connection).Select(projections);
        }

        #endregion

        #region Query

        public List<ORecord> Query(string sql)
        {
            return Query(sql, "*:0");
        }

        public List<ORecord> Query(string sql, string fetchPlan)
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

            return document.GetField<List<ORecord>>("Content");
        }

        #endregion

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
            if (_connection.IsReusable)
            {
                OClient.ReturnConnection(_connection);
            }
            else
            {
                _connection.Dispose();
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
