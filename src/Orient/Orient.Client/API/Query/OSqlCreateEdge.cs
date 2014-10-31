using System.Collections.Generic;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;
using Orient.Client.Protocol.Operations.Command;

// syntax: 
// CREATE EDGE [<class>] 
// [CLUSTER <cluster>] 
// FROM <rid>|(<query>)|[<rid>]* 
// TO <rid>|(<query>)|[<rid>]* 
// [SET <field> = <expression>[,]*]

namespace Orient.Client
{
    public interface OSqlCreateEdge
    {
        OSqlCreateEdge Edge(string className);
        OSqlCreateEdge Edge<T>(T obj);
        OSqlCreateEdge Edge<T>();
        OSqlCreateEdge Cluster(string clusterName);
        OSqlCreateEdge Cluster<T>();
        OSqlCreateEdge From(ORID orid);
        OSqlCreateEdge From<T>(T obj);
        OSqlCreateEdge To(ORID orid);
        OSqlCreateEdge To<T>(T obj);
        OSqlCreateEdge Set<T>(string fieldName, T fieldValue);
        OSqlCreateEdge Set<T>(T obj);
        OEdge Run();
        T Run<T>() where T : class, new();
        string ToString();
    }

    public class OSqlCreateEdgeViaSql : OSqlCreateEdge
    {
        private SqlQuery _sqlQuery = new SqlQuery();
        private Connection _connection;

        public OSqlCreateEdgeViaSql()
        {
        }

        internal OSqlCreateEdgeViaSql(Connection connection)
        {
            _connection = connection;
        }

        #region Edge

        public OSqlCreateEdge Edge(string className)
        {
            _sqlQuery.Edge(className);

            return this;
        }

        public OSqlCreateEdge Edge<T>(T obj)
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

            string className = document.OClassName;

            if (typeof(T) == typeof(OEdge))
            {
                className = "E";
            }
            else if (string.IsNullOrEmpty(document.OClassName))
            {
                throw new OException(OExceptionType.Query, "Document doesn't contain OClassName value.");
            }

            _sqlQuery.Edge(className);
            _sqlQuery.Set(document);

            return this;
        }

        public OSqlCreateEdge Edge<T>()
        {
            return Edge(typeof(T).Name);
        }

        #endregion

        #region Cluster

        public OSqlCreateEdge Cluster(string clusterName)
        {
            _sqlQuery.Cluster(clusterName);

            return this;
        }

        public OSqlCreateEdge Cluster<T>()
        {
            return Cluster(typeof(T).Name);
        }

        #endregion

        #region From

        public OSqlCreateEdge From(ORID orid)
        {
            _sqlQuery.From(orid);

            return this;
        }

        public OSqlCreateEdge From<T>(T obj)
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

            if (document.ORID == null)
            {
                throw new OException(OExceptionType.Query, "Document doesn't contain ORID value.");
            }

            _sqlQuery.From(document.ORID);

            return this;
        }

        #endregion

        #region To

        public OSqlCreateEdge To(ORID orid)
        {
            _sqlQuery.To(orid);

            return this;
        }

        public OSqlCreateEdge To<T>(T obj)
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
            
            if (document.ORID == null)
            {
                throw new OException(OExceptionType.Query, "Document doesn't contain ORID value.");
            }

            _sqlQuery.To(document.ORID);

            return this;
        }

        #endregion

        #region Set

        public OSqlCreateEdge Set<T>(string fieldName, T fieldValue)
        {
            _sqlQuery.Set<T>(fieldName, fieldValue);

            return this;
        }

        public OSqlCreateEdge Set<T>(T obj)
        {
            _sqlQuery.Set(obj);

            return this;
        }

        #endregion

        #region Run

        public OEdge Run()
        {
            CommandPayloadCommand payload = new CommandPayloadCommand();
            payload.Text = ToString();

            Command operation = new Command();
            operation.OperationMode = OperationMode.Synchronous;
            operation.CommandPayload = payload;

            OCommandResult result = new OCommandResult(_connection.ExecuteOperation(operation));

            return result.ToSingle().To<OEdge>();
        }

        public T Run<T>() where T : class, new()
        {
            return Run().To<T>();
        }

        #endregion

        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.CreateEdge);
        }
    }
}
