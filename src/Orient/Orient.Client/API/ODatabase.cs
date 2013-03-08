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
            CommandPayload payload = new CommandPayload();
            payload.Type = CommandPayloadType.Sql;
            payload.Text = sql;
            payload.NonTextLimit = -1;
            payload.FetchPlan = "*:0";
            payload.SerializedParams = new byte[] { 0 };

            Command operation = new Command();
            operation.OperationMode = OperationMode.Synchronous;
            operation.ClassType = CommandClassType.Idempotent;
            operation.CommandPayload = payload;

            DataObject dataObject = _connection.ExecuteOperation<Command>(operation);

            return dataObject.Get<List<ORecord>>("Content");
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
