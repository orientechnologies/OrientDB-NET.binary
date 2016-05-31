using Orient.Client.API.Query.Interfaces;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;
using Orient.Client.Protocol.Operations.Command;

// syntax: 
// CREATE VERTEX [<class>] 
// [CLUSTER <cluster>] 
// [SET <field> = <expression>[,]*]

namespace Orient.Client
{
    public class OSqlCreateVertex : IOCreateVertex
    {
        private SqlQuery _sqlQuery;
        private Connection _connection;

        public OSqlCreateVertex()
        {
            _sqlQuery = new SqlQuery(null);
        }
        internal OSqlCreateVertex(Connection connection)
        {
            _connection = connection;
            _sqlQuery = new SqlQuery(connection);
        }

        #region Vertex

        public IOCreateVertex Vertex(string className)
        {
            _sqlQuery.Vertex(className);

            return this;
        }

        public IOCreateVertex Vertex<T>(T obj)
        {
            ODocument document;

            if (obj is ODocument)
            {
                document = obj as ODocument;
            }
            else
            {
                document = ODocument.ToDocument(obj);
            }

            if (string.IsNullOrEmpty(document.OClassName))
            {
                throw new OException(OExceptionType.Query, "Document doesn't contain OClassName value.");
            }

            _sqlQuery.Vertex(document.OClassName);
            _sqlQuery.Set(document);

            return this;
        }

        public IOCreateVertex Vertex<T>()
        {
            return Vertex(typeof(T).Name);
        }

        #endregion

        #region Cluster

        public IOCreateVertex Cluster(string clusterName)
        {
            _sqlQuery.Cluster(clusterName);

            return this;
        }

        public IOCreateVertex Cluster<T>()
        {
            return Cluster(typeof(T).Name);
        }

        #endregion

        #region Set

        public IOCreateVertex Set<T>(string fieldName, T fieldValue)
        {
            _sqlQuery.Set<T>(fieldName, fieldValue);

            return this;
        }

        public IOCreateVertex Set<T>(T obj)
        {
            _sqlQuery.Set(obj);

            return this;
        }

        #endregion

        #region Run

        public OVertex Run()
        {
            CommandPayloadCommand payload = new CommandPayloadCommand();
            payload.Text = ToString();

            Command operation = new Command(_connection.Database);
            operation.OperationMode = OperationMode.Synchronous;
            operation.CommandPayload = payload;

            OCommandResult result = new OCommandResult(_connection.ExecuteOperation(operation));

            return result.ToSingle().To<OVertex>();
        }

        public T Run<T>() where T : class, new()
        {
            return Run().To<T>();
        }

        #endregion

        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.CreateVertex);
        }
    }
}
