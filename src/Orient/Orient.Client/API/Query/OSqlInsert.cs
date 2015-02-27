using Orient.Client.API.Query.Interfaces;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;
using Orient.Client.Protocol.Operations.Command;

// syntax:
// INSERT INTO <Class>|cluster:<cluster>|index:<index> 
// [<cluster>](cluster) 
// [VALUES (<expression>[,]((<field>[,]*))*)]|[<field> = <expression>[,](SET)*]

namespace Orient.Client
{
    public class OSqlInsert : IOInsert
    {
        private SqlQuery _sqlQuery;
        private Connection _connection;

        public OSqlInsert()
        {
            _sqlQuery = new SqlQuery(null);
        }

        internal OSqlInsert(Connection connection)
        {
            _connection = connection;
             _sqlQuery = new SqlQuery(connection);
        }

        #region Insert

        public IOInsert Insert(string className)
        {
            return Into(className);
        }

        public IOInsert Insert<T>()
        {
            return Into<T>();
        }

        public IOInsert Insert<T>(T obj)
        {
            // check for OClassName shouldn't have be here since INTO clause might specify it

            _sqlQuery.Insert(obj);

            return this;
        }

        #endregion

        #region Into

        public IOInsert Into(string className)
        {
            _sqlQuery.Class(className);

            return this;
        }

        public IOInsert Into<T>()
        {
            Into(typeof(T).Name);

            return this;
        }

        #endregion

        #region Cluster

        public IOInsert Cluster(string clusterName)
        {
            _sqlQuery.Cluster(clusterName);

            return this;
        }

        public IOInsert Cluster<T>()
        {
            return Cluster(typeof(T).Name);
        }

        #endregion

        #region Set

        public IOInsert Set<T>(string fieldName, T fieldValue)
        {
            _sqlQuery.Set<T>(fieldName, fieldValue);

            return this;
        }

        public IOInsert Set<T>(T obj)
        {
            _sqlQuery.Set(obj);

            return this;
        }

        #endregion

        #region Run

        public ODocument Run()
        {
            CommandPayloadCommand payload = new CommandPayloadCommand();
            payload.Text = ToString();

            Command operation = new Command(_connection.Database);
            operation.OperationMode = OperationMode.Synchronous;
            operation.CommandPayload = payload;

            OCommandResult result = new OCommandResult(_connection.ExecuteOperation(operation));

            return result.ToSingle();
        }

        public T Run<T>() where T : class, new() 
        {
            return Run().To<T>();
        }

        #endregion

        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.Insert);
        }
    }
}
