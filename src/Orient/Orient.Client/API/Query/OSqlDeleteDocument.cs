using System.Collections.Generic;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

// syntax:
// DELETE FROM <Class>|cluster:<cluster>|index:<index> 
// [<Condition>*](WHERE) 
// [BY <Fields>* [ASC|DESC](ORDER)*] 
// [<MaxRecords>](LIMIT)

namespace Orient.Client
{
    public class OSqlDelete
    {
        private SqlQuery _sqlQuery = new SqlQuery();
        private Connection _connection;

        public OSqlDelete()
        {
        }

        internal OSqlDelete(Connection connection)
        {
            _connection = connection;
        }

        public OSqlDelete Delete<T>(T obj)
        {
            _sqlQuery.Delete(obj);

            return this;
        }

        #region Class

        public OSqlDelete Class(string className)
        {
            _sqlQuery.Class(className);

            return this;
        }

        public OSqlDelete Class<T>()
        {
            return Class(typeof(T).Name);
        }

        #endregion

        #region Cluster

        public OSqlDelete Cluster(string clusterName)
        {
            _sqlQuery.Cluster("cluster:" + clusterName);

            return this;
        }

        public OSqlDelete Cluster<T>()
        {
            return Cluster(typeof(T).Name);
        }

        #endregion

        #region Record

        public OSqlDelete Record(ORID orid)
        {
            _sqlQuery.Record(orid);

            return this;
        }

        public OSqlDelete Record(ODocument document)
        {
            return Record(document.ORID);
        }

        #endregion

        #region Where with conditions

        public OSqlDelete Where(string field)
        {
            _sqlQuery.Where(field);

            return this;
        }

        public OSqlDelete And(string field)
        {
            _sqlQuery.And(field);

            return this;
        }

        public OSqlDelete Or(string field)
        {
            _sqlQuery.Or(field);

            return this;
        }

        public OSqlDelete Equals<T>(T item)
        {
            _sqlQuery.Equals<T>(item);

            return this;
        }

        public OSqlDelete NotEquals<T>(T item)
        {
            _sqlQuery.NotEquals<T>(item);

            return this;
        }

        public OSqlDelete Lesser<T>(T item)
        {
            _sqlQuery.Lesser<T>(item);

            return this;
        }

        public OSqlDelete LesserEqual<T>(T item)
        {
            _sqlQuery.LesserEqual<T>(item);

            return this;
        }

        public OSqlDelete Greater<T>(T item)
        {
            _sqlQuery.Greater<T>(item);

            return this;
        }

        public OSqlDelete GreaterEqual<T>(T item)
        {
            _sqlQuery.GreaterEqual<T>(item);

            return this;
        }

        public OSqlDelete Like<T>(T item)
        {
            _sqlQuery.Like<T>(item);

            return this;
        }

        public OSqlDelete IsNull()
        {
            _sqlQuery.IsNull();

            return this;
        }

        public OSqlDelete Contains<T>(T item)
        {
            _sqlQuery.Contains<T>(item);

            return this;
        }

        public OSqlDelete Contains<T>(string field, T value)
        {
            _sqlQuery.Contains<T>(field, value);

            return this;
        }

        #endregion

        public OSqlDelete Limit(int maxRecords)
        {
            _sqlQuery.Limit(maxRecords);

            return this;
        }

        public int Run()
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

            return int.Parse(result.ToDocument().GetField<string>("Content"));
        }

        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.Delete);
        }
    }
}
