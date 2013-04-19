using System.Collections.Generic;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

// syntax: CREATE VERTEX [<class>] [CLUSTER <cluster>] [SET <field> = <expression>[,]*]

namespace Orient.Client
{
    public class OSqlCreateVertex
    {
        private Connection _connection;
        private SqlQuery _sqlQuery;

        public OSqlCreateVertex()
        {
            _sqlQuery = new SqlQuery();
        }

        internal OSqlCreateVertex(Connection connection)
        {
            _connection = connection;
            _sqlQuery = new SqlQuery();
        }

        #region Vertex

        public OSqlCreateVertex Vertex(string className)
        {
            _sqlQuery.Join(Q.Create, Q.Vertex, className);

            return this;
        }

        public OSqlCreateVertex Vertex<T>()
        {
            return Vertex(typeof(T).Name);
        }

        public OSqlCreateVertex Vertex(ODocument document)
        {
            if (!document.HasField("@ClassName"))
            {
                throw new OException(OExceptionType.Query, "Document doesn't contain @ClassName field which identifies vertex class.");
            }

            Vertex(document.GetField<string>("@ClassName"));

            return Set(document);
        }

        #endregion

        #region Cluster

        public OSqlCreateVertex Cluster(string clusterName)
        {
            _sqlQuery.Join("", Q.Cluster, clusterName);

            return this;
        }

        public OSqlCreateVertex Cluster<T>()
        {
            return Cluster(typeof(T).Name);
        }

        #endregion

        #region Set

        public OSqlCreateVertex Set<T>(string fieldName, T fieldValue)
        {
            _sqlQuery.SetField<T>(fieldName, fieldValue);

            return this;
        }

        public OSqlCreateVertex Set<T>(T obj)
        {
            _sqlQuery.SetFields(obj);

            return this;
        }

        #endregion

        public ODocument Run()
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
