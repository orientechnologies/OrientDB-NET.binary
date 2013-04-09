using System.Collections.Generic;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

// syntax:
// INSERT INTO <Class>|cluster:<cluster>|index:<index> [<cluster>](cluster) 
// [VALUES (<expression>[,]((<field>[,]*))*)]|[<field> = <expression>[,](SET)*]

namespace Orient.Client
{
    public class OSqlInsert
    {
        private Connection _connection;
        private SqlQuery _sqlQuery;

        public OSqlInsert()
        {
            _sqlQuery = new SqlQuery();
        }

        internal OSqlInsert(Connection connection)
        {
            _connection = connection;
            _sqlQuery = new SqlQuery();
        }

        #region Into

        public OSqlInsert Into(string className)
        {
            _sqlQuery.Join(Q.Insert, Q.Into, className);

            return this;
        }

        public OSqlInsert Into<T>()
        {
            Into(typeof(T).Name);

            return this;
        }

        #endregion

        #region Cluster

        public OSqlInsert Cluster(string clusterName)
        {
            _sqlQuery.Join("", Q.Cluster, clusterName);

            return this;
        }

        public OSqlInsert Cluster<T>()
        {
            return Cluster(typeof(T).Name);
        }

        #endregion

        #region Set

        public OSqlInsert Set<T>(string fieldName, T fieldValue)
        {
            _sqlQuery.SetField<T>(fieldName, fieldValue);

            return this;
        }

        public OSqlInsert Set<T>(T obj)
        {
            _sqlQuery.SetFields(obj);

            return this;
        }

        #endregion

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

        public override string ToString()
        {
            return _sqlQuery.ToString();
        }
    }
}
