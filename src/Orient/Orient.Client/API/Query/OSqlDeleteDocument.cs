using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;
using Orient.Client.Protocol.Operations.Command;

// syntax:
// DELETE FROM <Class>|cluster:<cluster>|index:<index> 
// [<Condition>*](WHERE) 
// [BY <Fields>* [ASC|DESC](ORDER)*] 
// [<MaxRecords>](LIMIT)

namespace Orient.Client
{
    public class OSqlDeleteDocument
    {
        private SqlQuery _sqlQuery;
        private Connection _connection;

        public OSqlDeleteDocument()
        {
            _sqlQuery = new SqlQuery(null);
        }
        internal OSqlDeleteDocument(Connection connection)
        {
            _connection = connection;
            _sqlQuery = new SqlQuery(connection);
        }

        public OSqlDeleteDocument Delete<T>(T obj)
        {
            _sqlQuery.Delete(obj);

            return this;
        }

        #region Class

        public OSqlDeleteDocument Class(string className)
        {
            _sqlQuery.Class(className);

            return this;
        }

        public OSqlDeleteDocument Class<T>()
        {
            return Class(typeof(T).Name);
        }

        #endregion

        #region Cluster

        public OSqlDeleteDocument Cluster(string clusterName)
        {
            _sqlQuery.Cluster("cluster:" + clusterName);

            return this;
        }

        public OSqlDeleteDocument Cluster<T>()
        {
            return Cluster(typeof(T).Name);
        }

        #endregion

        #region Record

        public OSqlDeleteDocument Record(ORID orid)
        {
            _sqlQuery.Record(orid);

            return this;
        }

        public OSqlDeleteDocument Record(ODocument document)
        {
            return Record(document.ORID);
        }

        #endregion

        #region Where with conditions

        public OSqlDeleteDocument Where(string field)
        {
            _sqlQuery.Where(field);

            return this;
        }

        public OSqlDeleteDocument And(string field)
        {
            _sqlQuery.And(field);

            return this;
        }

        public OSqlDeleteDocument Or(string field)
        {
            _sqlQuery.Or(field);

            return this;
        }

        public OSqlDeleteDocument Equals<T>(T item)
        {
            _sqlQuery.Equals<T>(item);

            return this;
        }

        public OSqlDeleteDocument NotEquals<T>(T item)
        {
            _sqlQuery.NotEquals<T>(item);

            return this;
        }

        public OSqlDeleteDocument Lesser<T>(T item)
        {
            _sqlQuery.Lesser<T>(item);

            return this;
        }

        public OSqlDeleteDocument LesserEqual<T>(T item)
        {
            _sqlQuery.LesserEqual<T>(item);

            return this;
        }

        public OSqlDeleteDocument Greater<T>(T item)
        {
            _sqlQuery.Greater<T>(item);

            return this;
        }

        public OSqlDeleteDocument GreaterEqual<T>(T item)
        {
            _sqlQuery.GreaterEqual<T>(item);

            return this;
        }

        public OSqlDeleteDocument Like<T>(T item)
        {
            _sqlQuery.Like<T>(item);

            return this;
        }

        public OSqlDeleteDocument IsNull()
        {
            _sqlQuery.IsNull();

            return this;
        }

        public OSqlDeleteDocument Contains<T>(T item)
        {
            _sqlQuery.Contains<T>(item);

            return this;
        }

        public OSqlDeleteDocument Contains<T>(string field, T value)
        {
            _sqlQuery.Contains<T>(field, value);

            return this;
        }

        #endregion

        public OSqlDeleteDocument Limit(int maxRecords)
        {
            _sqlQuery.Limit(maxRecords);

            return this;
        }

        public int Run()
        {
            CommandPayloadCommand payload = new CommandPayloadCommand();
            payload.Text = ToString();

            Command operation = new Command(_connection.Database);
            operation.OperationMode = OperationMode.Synchronous;
            operation.CommandPayload = payload;

            OCommandResult result = new OCommandResult(_connection.ExecuteOperation(operation));

            return int.Parse(result.ToDocument().GetField<string>("Content"));
        }

        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.DeleteDocument);
        }
    }
}
