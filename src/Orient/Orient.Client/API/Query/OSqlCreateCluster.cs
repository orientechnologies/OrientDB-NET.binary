using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;
using Orient.Client.Protocol.Operations.Command;

// syntax:
// CREATE CLUSTER <name> <type> 
// [DATASEGMENT <data-segment>|default] 
// [LOCATION <path>|default] 
// [POSITION <position>|append]

namespace Orient.Client
{
    public interface OSqlCreateCluster {
        OSqlCreateCluster Cluster(string clusterName, OClusterType clusterType);
        OSqlCreateCluster Cluster<T>(OClusterType clusterType);
        short Run();
        string ToString();
    }

    public class OSqlCreateClusterViaSql : OSqlCreateCluster
    {
        private SqlQuery _sqlQuery = new SqlQuery();
        private Connection _connection;

        public OSqlCreateClusterViaSql()
        {
        }

        internal OSqlCreateClusterViaSql(Connection connection)
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
            CommandPayloadCommand payload = new CommandPayloadCommand();
            payload.Text = ToString();

            Command operation = new Command();
            operation.OperationMode = OperationMode.Synchronous;
            operation.CommandPayload = payload;

            OCommandResult result = new OCommandResult(_connection.ExecuteOperation(operation));

            return short.Parse(result.ToDocument().GetField<string>("Content"));
        }

        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.CreateCluster);
        }
    }
}
