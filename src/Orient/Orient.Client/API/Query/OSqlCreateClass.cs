using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

// syntax: CREATE CLASS <class> [EXTENDS <super-class>] [CLUSTER <clusterId>*]

namespace Orient.Client
{
    public class OSqlCreateClass
    {
        private Connection _connection;
        private SqlQuery _sqlQuery;

        internal OSqlCreateClass(Connection connection, string className)
        {
            _connection = connection;
            _sqlQuery = new SqlQuery(Q.Create, Q.Class, className);
        }

        public OSqlCreateClass Extends<T>()
        {
            return Extends(typeof(T).Name);
        }

        public OSqlCreateClass Extends(string superClass)
        {
            _sqlQuery.Join("", Q.Extends, superClass);

            return this;
        }

        public OSqlCreateClass Cluster(short clusterId)
        {
            _sqlQuery.Join("", Q.Cluster, clusterId.ToString());

            return this;
        }

        public short Run()
        {
            CommandPayload payload = new CommandPayload();
            payload.Type = CommandPayloadType.Sql;
            payload.Text = _sqlQuery.Value;
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
