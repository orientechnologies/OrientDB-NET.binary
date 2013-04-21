using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

// syntax: 
// CREATE CLASS <class> 
// [EXTENDS <super-class>] 
// [CLUSTER <clusterId>*]

namespace Orient.Client
{
    public class OSqlCreateClass
    {
        private SqlQuery _sqlQuery = new SqlQuery();
        private SqlQuery2 _sqlQuery2 = new SqlQuery2();
        private Connection _connection;

        public OSqlCreateClass()
        {
        }

        internal OSqlCreateClass(Connection connection)
        {
            _connection = connection;
        }

        #region Class

        public OSqlCreateClass Class(string className)
        {
            _sqlQuery2.Class(className);

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
            _sqlQuery2.Extends(superClass);

            return this;
        }

        public OSqlCreateClass Extends<T>()
        {
            return Extends(typeof(T).Name);
        }

        #endregion

        public OSqlCreateClass Cluster(short clusterId)
        {
            _sqlQuery2.Cluster(clusterId.ToString());

            return this;
        }

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
            return _sqlQuery2.ToString(QueryType.CreateClass);
        }
    }
}
