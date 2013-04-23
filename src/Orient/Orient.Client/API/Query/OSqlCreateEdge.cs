using System.Collections.Generic;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

// syntax: 
// CREATE EDGE [<class>] 
// [CLUSTER <cluster>] 
// FROM <rid>|(<query>)|[<rid>]* 
// TO <rid>|(<query>)|[<rid>]* 
// [SET <field> = <expression>[,]*]

namespace Orient.Client
{
    public class OSqlCreateEdge
    {
        private SqlQuery _sqlQuery = new SqlQuery();
        private Connection _connection;

        public OSqlCreateEdge()
        {
        }

        internal OSqlCreateEdge(Connection connection)
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

            if (string.IsNullOrEmpty(document.OClassName))
            {
                throw new OException(OExceptionType.Query, "Document doesn't contain OClassName value.");
            }

            _sqlQuery.Edge(document.OClassName);
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

        public OSqlCreateEdge From(ODocument document)
        {
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

        public OSqlCreateEdge To(ODocument document)
        {
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

        public ODocument Run()
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

            return result.ToSingle();
        }

        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.CreateEdge);
        }
    }
}
