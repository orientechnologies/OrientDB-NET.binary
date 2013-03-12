using System;
using System.Collections.Generic;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

namespace Orient.Client
{
    public class ODatabase : IDisposable
    {
        private Connection _connection;

        public ODatabase(string alias)
        {
            _connection = OClient.ReleaseConnection(alias);
        }

        public List<OCluster> GetClusters()
        {
            return _connection.DataObject.Get<List<OCluster>>("Clusters");
        }

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
            operation.OperationMode = OperationMode.Synchronous;
            operation.ClassType = CommandClassType.Idempotent;
            operation.CommandPayload = payload;

            DataObject dataObject = _connection.ExecuteOperation<Command>(operation);

            return dataObject.Get<List<ORecord>>("Content");
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

            DataObject dataObject = _connection.ExecuteOperation<Command>(operation);
            
            return new OCommandResult(dataObject);
        }

        public void Close()
        {
            if (_connection.IsReusable)
            {
                OClient.ReturnConnection(_connection);
            }
            else
            {
                Close();

                _connection.Dispose();
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
