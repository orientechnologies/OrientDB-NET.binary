using System.Collections.Generic;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

// syntax: CREATE EDGE [<class>] [CLUSTER <cluster>] FROM <rid>|(<query>)|[<rid>]* TO <rid>|(<query>)|[<rid>]* [SET <field> = <expression>[,]*]

namespace Orient.Client
{
    public class OSqlCreateEdge
    {
        private Connection _connection;
        private SqlQuery _sqlQuery;

        public OSqlCreateEdge()
        {
            _sqlQuery = new SqlQuery();
        }

        internal OSqlCreateEdge(Connection connection)
        {
            _connection = connection;
            _sqlQuery = new SqlQuery();
        }

        #region Edge

        public OSqlCreateEdge Edge(string className)
        {
            _sqlQuery.Join(Q.Create, Q.Edge, className);

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
            _sqlQuery.Join("", Q.Cluster, clusterName);

            return this;
        }

        public OSqlCreateEdge Cluster<T>()
        {
            return Cluster(typeof(T).Name);
        }

        #endregion

        #region From

        public OSqlCreateEdge From(ORID from)
        {
            _sqlQuery.Join("", Q.From, from.ToString());

            return this;
        }

        public OSqlCreateEdge From(ODocument document)
        {
            if (!document.HasField("@ORID"))
            {
                throw new OException(OExceptionType.Query, "Document doesn't contain @ORID field which identifies FROM vertex.");
            }

            return From(document.GetField<ORID>("@ORID"));
        }

        #endregion

        #region To

        public OSqlCreateEdge To(ORID to)
        {
            _sqlQuery.Join("", Q.To, to.ToString());

            return this;
        }

        public OSqlCreateEdge To(ODocument document)
        {
            if (!document.HasField("@ORID"))
            {
                throw new OException(OExceptionType.Query, "Document doesn't contain @ORID field which identifies TO vertex.");
            }

            return To(document.GetField<ORID>("@ORID"));
        }

        #endregion

        #region Set

        public OSqlCreateEdge Set<T>(string fieldName, T fieldValue)
        {
            _sqlQuery.SetField<T>(fieldName, fieldValue);

            return this;
        }

        public OSqlCreateEdge Set<T>(T obj)
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
