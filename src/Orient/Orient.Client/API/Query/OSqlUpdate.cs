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
        private Connection _connection;
        private SqlQuery _sqlQuery;
        private bool _hasAdd;
        private bool _hasRemove;

        public OSqlUpdate()
        {
            _sqlQuery = new SqlQuery();
        }

        internal OSqlUpdate(Connection connection)
        {
            _connection = connection;
            _sqlQuery = new SqlQuery();
        }

        #region Class

        public OSqlUpdate Class(string className)
        {
            _sqlQuery.Join(Q.Update, className);

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
            _sqlQuery.Join(Q.Update, "cluster:" + clusterName);

            return this;
        }

        public OSqlUpdate Cluster<T>()
        {
            return Cluster(typeof(T).Name);
        }

        #endregion

        public OSqlUpdate Record(ORID orid)
        {
            _sqlQuery.Join(Q.Update, orid.ToString());

            return this;
        }

        #region Set

        public OSqlUpdate Set<T>(string fieldName, T fieldValue)
        {
            _sqlQuery.SetField<T>(fieldName, fieldValue);

            return this;
        }

        public OSqlUpdate Set<T>(T obj)
        {
            _sqlQuery.SetFields(obj);

            return this;
        }

        #endregion

        #region Add

        public OSqlUpdate Add<T>(string fieldName, T fieldValue)
        {
            if (!_hasAdd)
            {
                _hasAdd = true;
                _sqlQuery.Join("", Q.Add);
            }
            else
            {
                _sqlQuery.Join(Q.Comma);
            }

            _sqlQuery.Join("", fieldName, Q.Equals, SqlQuery.ToString(fieldValue));

            return this;
        }

        #endregion

        #region Remove

        public OSqlUpdate Remove(string fieldName)
        {
            if (!_hasRemove)
            {
                _hasRemove = true;
                _sqlQuery.Join("", Q.Remove);
            }
            else
            {
                _sqlQuery.Join(Q.Comma);
            }

            _sqlQuery.Join("", fieldName);

            return this;
        }

        public OSqlUpdate Remove<T>(string fieldName, T collectionValue)
        {
            if (!_hasRemove)
            {
                _hasRemove = true;
                _sqlQuery.Join("", Q.Remove);
            }
            else
            {
                _sqlQuery.Join(Q.Comma);
            }

            _sqlQuery.Join("", fieldName, Q.Equals, SqlQuery.ToString(collectionValue));

            return this;
        }

        #endregion

        #region Where with conditions

        public OSqlUpdate Where(string field)
        {
            _sqlQuery.Where(field);

            return this;
        }

        public OSqlUpdate And(string field)
        {
            _sqlQuery.And(field);

            return this;
        }

        public OSqlUpdate Or(string field)
        {
            _sqlQuery.Or(field);

            return this;
        }

        public OSqlUpdate Equals<T>(T item)
        {
            _sqlQuery.Equals<T>(item);

            return this;
        }

        public OSqlUpdate NotEquals<T>(T item)
        {
            _sqlQuery.NotEquals<T>(item);

            return this;
        }

        public OSqlUpdate Lesser<T>(T item)
        {
            _sqlQuery.Lesser<T>(item);

            return this;
        }

        public OSqlUpdate LesserEqual<T>(T item)
        {
            _sqlQuery.LesserEqual<T>(item);

            return this;
        }

        public OSqlUpdate Greater<T>(T item)
        {
            _sqlQuery.Greater<T>(item);

            return this;
        }

        public OSqlUpdate GreaterEqual<T>(T item)
        {
            _sqlQuery.GreaterEqual<T>(item);

            return this;
        }

        public OSqlUpdate Like<T>(T item)
        {
            _sqlQuery.Like<T>(item);

            return this;
        }

        public OSqlUpdate IsNull()
        {
            _sqlQuery.IsNull();

            return this;
        }

        public OSqlUpdate Contains<T>(T item)
        {
            _sqlQuery.Contains<T>(item);

            return this;
        }

        public OSqlUpdate Contains<T>(string field, T value)
        {
            _sqlQuery.Contains<T>(field, value);

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
            payload.Text = _sqlQuery.ToString();
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
            return _sqlQuery.ToString();
        }
    }
}
