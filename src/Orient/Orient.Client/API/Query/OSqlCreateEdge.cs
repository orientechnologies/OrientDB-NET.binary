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
        private SqlQuery2 _sqlQuery2 = new SqlQuery2();
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
            _sqlQuery2.Edge(className);

            return this;
        }

        public OSqlCreateEdge Edge(ODocument document)
        {
            if (string.IsNullOrEmpty(document.OClassName))
            {
                throw new OException(OExceptionType.Query, "Document doesn't contain OClassName value.");
            }

            _sqlQuery2.Edge(document.OClassName);
            _sqlQuery2.Set(document);

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
            _sqlQuery2.Cluster(clusterName);

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
            _sqlQuery2.From(orid);

            return this;
        }

        public OSqlCreateEdge From(ODocument document)
        {
            if (document.ORID == null)
            {
                throw new OException(OExceptionType.Query, "Document doesn't contain ORID value.");
            }

            _sqlQuery2.From(document.ORID);

            return this;
        }

        #endregion

        #region To

        public OSqlCreateEdge To(ORID orid)
        {
            _sqlQuery2.To(orid);

            return this;
        }

        public OSqlCreateEdge To(ODocument document)
        {
            if (document.ORID == null)
            {
                throw new OException(OExceptionType.Query, "Document doesn't contain ORID value.");
            }

            _sqlQuery2.To(document.ORID);

            return this;
        }

        #endregion

        #region Set

        public OSqlCreateEdge Set<T>(string fieldName, T fieldValue)
        {
            _sqlQuery2.Set<T>(fieldName, fieldValue);

            return this;
        }

        public OSqlCreateEdge Set<T>(T obj)
        {
            _sqlQuery2.Set(obj);

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
            return _sqlQuery2.ToString(QueryType.CreateEdge);
        }
    }
}
