using System.Collections.Generic;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

// syntax: CREATE VERTEX [<class>] [CLUSTER <cluster>] [SET <field> = <expression>[,]*]

namespace Orient.Client
{
    public class OSqlCreateVertex
    {
        private Connection _connection;
        private SqlQuery _sqlQuery;

        internal OSqlCreateVertex(Connection connection, string className)
        {
            _connection = connection;
            _sqlQuery = new SqlQuery(Q.Create, Q.Vertex, className);
        }

        public OSqlCreateVertex Cluster(string clusterName)
        {
            _sqlQuery.Join("", Q.Cluster, clusterName);

            return this;
        }

        public OSqlCreateVertex Set<T>(string fieldName, T fieldValue)
        {
            _sqlQuery.SetField<T>(fieldName, fieldValue);

            return this;
        }

        public OSqlCreateVertex Set<T>(T obj)
        {
            _sqlQuery.SetFields(obj);

            return this;
        }

        public ORecord Run()
        {
            CommandPayload payload = new CommandPayload();
            payload.Type = CommandPayloadType.Sql;
            payload.Text = _sqlQuery.ToString();
            payload.NonTextLimit = -1;
            payload.FetchPlan = "";
            payload.SerializedParams = new byte[] { 0 };

            Command operation = new Command();
            operation.OperationMode = OperationMode.Synchronous;
            operation.ClassType = CommandClassType.NonIdempotent;
            operation.CommandPayload = payload;

            OCommandResult result = new OCommandResult(_connection.ExecuteOperation<Command>(operation));

            return result.ToSingle();
        }
    }
}
