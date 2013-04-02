using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

// syntax: CREATE CLASS <class> [EXTENDS <super-class>] [CLUSTER <clusterId>*]

namespace Orient.Client
{
    public class OSqlCreateClass
    {
        private Connection _connection;
        private SqlQuery _sqlQuery;

        public OSqlCreateClass()
        {
            _sqlQuery = new SqlQuery();
        }

        internal OSqlCreateClass(Connection connection)
        {
            _connection = connection;
            _sqlQuery = new SqlQuery();
        }

        #region Class

        public OSqlCreateClass Class(string className)
        {
            _sqlQuery.Join(Q.Create, Q.Class, className);

            return this;
        }

        public OSqlCreateClass Class<T>()
        {
            return Class(typeof(T).Name);
        }

        #endregion

        #region Extends

        public OSqlCreateClass Extends(string superClass)
        {
            _sqlQuery.Join("", Q.Extends, superClass);

            return this;
        }

        public OSqlCreateClass Extends<T>()
        {
            return Extends(typeof(T).Name);
        }

        #endregion

        public OSqlCreateClass Cluster(short clusterId)
        {
            _sqlQuery.Join("", Q.Cluster, clusterId.ToString());

            return this;
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

        public override string ToString()
        {
            return _sqlQuery.ToString();
        }
    }
}
