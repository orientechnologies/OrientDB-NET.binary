using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

// syntax CREATE CLUSTER <name> <type> [DATASEGMENT <data-segment>|default] [LOCATION <path>|default] [POSITION <position>|append]

namespace Orient.Client
{
    public class OSqlCreateCluster
    {
        private Connection _connection;
        private SqlQuery _sqlQuery;

        internal OSqlCreateCluster(Connection connection, string clusterName, OClusterType clusterType)
        {
            _connection = connection;
            _sqlQuery = new SqlQuery(Q.Create, Q.Cluster, clusterName, clusterType.ToString().ToUpper());
        }

        public short Run()
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

            return short.Parse(result.ToDataObject().Get<string>("Content"));
        }
    }
}
