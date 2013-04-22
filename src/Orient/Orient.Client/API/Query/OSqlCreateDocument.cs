using System.Collections.Generic;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

// shorthand for INSERT INTO for documents

namespace Orient.Client
{
    public class OSqlCreateDocument
    {
        private SqlQuery _sqlQuery = new SqlQuery();
        private Connection _connection;

        public OSqlCreateDocument()
        {
        }

        internal OSqlCreateDocument(Connection connection)
        {
            _connection = connection;
        }

        #region Document

        public OSqlCreateDocument Document(string className)
        {
            _sqlQuery.Class(className);

            return this;
        }

        public OSqlCreateDocument Document(ODocument document)
        {
            // check for OClassName shouldn't have be here since INTO clause might specify it

            _sqlQuery.Insert(document);

            return this;
        }

        public OSqlCreateDocument Document<T>()
        {
            return Document(typeof(T).Name);
        }

        #endregion

        #region Cluster

        public OSqlCreateDocument Cluster(string clusterName)
        {
            _sqlQuery.Cluster(clusterName);

            return this;
        }

        public OSqlCreateDocument Cluster<T>()
        {
            return Cluster(typeof(T).Name);
        }

        #endregion

        #region Set

        public OSqlCreateDocument Set<T>(string fieldName, T fieldValue)
        {
            _sqlQuery.Set<T>(fieldName, fieldValue);

            return this;
        }

        public OSqlCreateDocument Set<T>(T obj)
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
            return _sqlQuery.ToString(QueryType.Insert);
        }
    }
}
