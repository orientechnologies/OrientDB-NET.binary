using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

// syntax:
// CREATE CLUSTER <name> <type> 
// [DATASEGMENT <data-segment>|default] 
// [LOCATION <path>|default] 
// [POSITION <position>|append]

namespace Orient.Client
{
    public class OSqlCreateCluster
    {
        private SqlQuery _sqlQuery = new SqlQuery();
        private Connection _connection;

        public OSqlCreateCluster()
        {
        }

        internal OSqlCreateCluster(Connection connection)
        {
            _connection = connection;
        }

        #region Cluster

        public OSqlCreateCluster Cluster(string clusterName, OClusterType clusterType)
        {
            _sqlQuery.Cluster(clusterName, clusterType);

            return this;
        }

        public OSqlCreateCluster Cluster<T>(OClusterType clusterType)
        {
            return Cluster(typeof(T).Name, clusterType);
        }

        #endregion

        public short Run()
        {
            CommandPayload payload = new CommandPayload();
            payload.Type = CommandPayloadType.Sql;
            payload.Text = ToString();
            payload.NonTextLimit = -1;
            payload.FetchPlan = "";
            payload.SerializedParams = new byte[] { 0 };

            Command operation = new Command();
            operation.OperationMode = OperationMode.Synchronous;
            operation.ClassType = CommandClassType.NonIdempotent;
            operation.CommandPayload = payload;

            OCommandResult result = new OCommandResult(_connection.ExecuteOperation<Command>(operation));

            return short.Parse(result.ToDocument().GetField<string>("Content"));
        }

        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.CreateCluster);
        }
    }
}
