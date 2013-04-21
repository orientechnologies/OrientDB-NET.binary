using System.Collections.Generic;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

// syntax: 
// UPDATE <class>|cluster:<cluster>> 
// SET|INCREMENT [= <field-value>](<field-name>)[<field-name> = <field-value>](,)* 
// [<conditions>] (WHERE) 
// [<max-records>](LIMIT)

// collections: 
// UPDATE <class>|cluster:<cluster>> 
// [[<field-name> = <field-value>](ADD|REMOVE])[<field-name> = <field-value>](,)* 
// [<conditions>](WHERE)

// maps:
// UPDATE <class>|cluster:<cluster>> 
// [[<field-name> = <map-key> [,<map-value>]](PUT|REMOVE])[<field-name> = <map-key> [,<map-value>]](,)* 
// [<conditions>](WHERE)

namespace Orient.Client
{
    public class OSqlUpdate
    {
        private SqlQuery _sqlQuery = new SqlQuery();
        private SqlQuery2 _sqlQuery2 = new SqlQuery2();
        private Connection _connection;

        public OSqlUpdate()
        {
        }

        internal OSqlUpdate(Connection connection)
        {
            _connection = connection;
        }

        #region Class

        public OSqlUpdate Class(string className)
        {
            _sqlQuery2.Class(className);

            return this;
        }

        public OSqlUpdate Class<T>()
        {
            return Class(typeof(T).Name);
        }

        #endregion

        #region Cluster

        public OSqlUpdate Cluster(string clusterName)
        {
            _sqlQuery2.Cluster("cluster:" + clusterName);

            return this;
        }

        public OSqlUpdate Cluster<T>()
        {
            return Cluster(typeof(T).Name);
        }

        #endregion

        #region Record

        public OSqlUpdate Record(ORID orid)
        {
            //_sqlQuery.Join(Q.Update, orid.ToString());
            _sqlQuery2.Record(orid);

            return this;
        }

        public OSqlUpdate Record(ODocument document)
        {
            return Record(document.ORID);
        }

        #endregion

        public OSqlUpdate Document(ODocument document)
        {
            if (!string.IsNullOrEmpty(document.OClassName))
            {
                _sqlQuery2.Class(document.OClassName);
            }

            if (document.ORID != null)
            {
                _sqlQuery2.Record(document.ORID);
            }

            _sqlQuery2.Set(document);

            return this;
        }

        #region Set

        public OSqlUpdate Set<T>(string fieldName, T fieldValue)
        {
            _sqlQuery2.Set<T>(fieldName, fieldValue);

            return this;
        }

        public OSqlUpdate Set<T>(T obj)
        {
            _sqlQuery2.Set(obj);

            return this;
        }

        #endregion

        public OSqlUpdate Add<T>(string fieldName, T fieldValue)
        {
            _sqlQuery2.Add(fieldName, fieldValue);

            return this;
        }

        #region Remove

        public OSqlUpdate Remove(string fieldName)
        {
            _sqlQuery2.Remove(fieldName);

            return this;
        }

        public OSqlUpdate Remove<T>(string fieldName, T collectionValue)
        {
            _sqlQuery2.Remove(fieldName, collectionValue);

            return this;
        }

        #endregion

        #region Where with conditions

        public OSqlUpdate Where(string field)
        {
            //_sqlQuery.Where(field);
            _sqlQuery2.Where(field);

            return this;
        }

        public OSqlUpdate And(string field)
        {
            _sqlQuery2.And(field);

            return this;
        }

        public OSqlUpdate Or(string field)
        {
            _sqlQuery2.Or(field);

            return this;
        }

        public OSqlUpdate Equals<T>(T item)
        {
            _sqlQuery2.Equals<T>(item);

            return this;
        }

        public OSqlUpdate NotEquals<T>(T item)
        {
            _sqlQuery2.NotEquals<T>(item);

            return this;
        }

        public OSqlUpdate Lesser<T>(T item)
        {
            _sqlQuery2.Lesser<T>(item);

            return this;
        }

        public OSqlUpdate LesserEqual<T>(T item)
        {
            _sqlQuery2.LesserEqual<T>(item);

            return this;
        }

        public OSqlUpdate Greater<T>(T item)
        {
            _sqlQuery2.Greater<T>(item);

            return this;
        }

        public OSqlUpdate GreaterEqual<T>(T item)
        {
            _sqlQuery2.GreaterEqual<T>(item);

            return this;
        }

        public OSqlUpdate Like<T>(T item)
        {
            _sqlQuery2.Like<T>(item);

            return this;
        }

        public OSqlUpdate IsNull()
        {
            _sqlQuery2.IsNull();

            return this;
        }

        public OSqlUpdate Contains<T>(T item)
        {
            _sqlQuery2.Contains<T>(item);

            return this;
        }

        public OSqlUpdate Contains<T>(string field, T value)
        {
            _sqlQuery2.Contains<T>(field, value);

            return this;
        }

        #endregion

        public OSqlUpdate Limit(int maxRecords)
        {
            _sqlQuery.Join("", Q.Limit, maxRecords.ToString());

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
            return _sqlQuery2.ToString(QueryType.Update);
        }
    }
}
