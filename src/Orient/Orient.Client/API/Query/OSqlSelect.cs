using System.Collections.Generic;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

// SELECT [FROM <Target> [LET <Assignment>*](<Projections>]) [<Condition>*](WHERE) [BY <Field>](GROUP) [BY <Fields>* [ASC|DESC](ORDER)*] [<SkipRecords>](SKIP) [<MaxRecords>](LIMIT)

namespace Orient.Client
{
    public class OSqlSelect
    {
        private Connection _connection;
        private SqlQuery _sqlQuery;

        internal OSqlSelect(Connection connection, string sql)
        {
            _connection = connection;
            _sqlQuery = new SqlQuery(sql);
        }

        #region SELECT statement

        public OSqlSelect Also(string projection)
        {
            _sqlQuery.Join(Q.Comma, projection);

            return this;
        }

        public OSqlSelect First()
        {
            _sqlQuery.Surround("first");

            return this;
        }

        public OSqlSelect As(string alias)
        {
            _sqlQuery.Join("", Q.As, alias);

            return this;
        }

        #endregion

        #region FROM statement

        public OSqlSelect From<T>()
        {
            return From(typeof(T).Name);
        }

        public OSqlSelect From(ORID orid)
        {
            return From(orid.ToString());
        }

        public OSqlSelect From(string target)
        {
            _sqlQuery.Join("", Q.From, target);

            return this;
        }

        #endregion

        #region WHERE statement

        public OSqlSelect Where(string field)
        {
            _sqlQuery.Join("", Q.Where, field);

            return this;
        }

        public OSqlSelect And(string field)
        {
            _sqlQuery.Join("", Q.And, field);

            return this;
        }

        public OSqlSelect Or(string field)
        {
            _sqlQuery.Join("", Q.Or, field);

            return this;
        }

        public OSqlSelect Equals<T>(T item)
        {
            _sqlQuery.Join("", Q.Equals, SqlQuery.ToString(item));

            return this;
        }

        public OSqlSelect NotEquals<T>(T item)
        {
            _sqlQuery.Join("", Q.NotEquals, SqlQuery.ToString(item));

            return this;
        }

        public OSqlSelect Lesser<T>(T item)
        {
            _sqlQuery.Join("", Q.Lesser, SqlQuery.ToString(item));

            return this;
        }

        public OSqlSelect LesserEqual<T>(T item)
        {
            _sqlQuery.Join("", Q.LesserEqual, SqlQuery.ToString(item));

            return this;
        }

        public OSqlSelect Greater<T>(T item)
        {
            _sqlQuery.Join("", Q.Greater, SqlQuery.ToString(item));

            return this;
        }

        public OSqlSelect GreaterEqual<T>(T item)
        {
            _sqlQuery.Join("", Q.GreaterEqual, SqlQuery.ToString(item));

            return this;
        }

        public OSqlSelect Like<T>(T item)
        {
            _sqlQuery.Join("", Q.Like, SqlQuery.ToString(item));

            return this;
        }

        public OSqlSelect IsNull()
        {
            _sqlQuery.Join("", Q.Is, Q.Null);

            return this;
        }

        public OSqlSelect Contains<T>(T item)
        {
            _sqlQuery.Join("", Q.Contains, SqlQuery.ToString(item));

            return this;
        }

        public OSqlSelect Contains<T>(string field, T value)
        {
            _sqlQuery.Join("", Q.Contains, "(" + field, Q.Equals, SqlQuery.ToString(value) + ")");

            return this;
        }

        #endregion

        #region Run overloads

        public List<T> Run<T>() where T: class, new()
        {
            List<T> result = new List<T>();
            List<ORecord> records = Run("*:0");

            foreach (ORecord record in records)
            {
                result.Add(record.To<T>());
            }

            return result;
        }

        public List<ORecord> Run()
        {
            return Run("*:0");
        }

        public List<ORecord> Run(string fetchPlan)
        {
            CommandPayload payload = new CommandPayload();
            payload.Type = CommandPayloadType.Sql;
            payload.Text = _sqlQuery.ToString();
            payload.NonTextLimit = -1;
            payload.FetchPlan = fetchPlan;
            payload.SerializedParams = new byte[] { 0 };

            Command operation = new Command();
            operation.OperationMode = OperationMode.Asynchronous;
            operation.ClassType = CommandClassType.Idempotent;
            operation.CommandPayload = payload;

            OCommandResult commandResult = new OCommandResult(_connection.ExecuteOperation<Command>(operation));

            return commandResult.ToList();
        }

        #endregion

        public override string ToString()
        {
            return _sqlQuery.ToString();
        }
    }
}
