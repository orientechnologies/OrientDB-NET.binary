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

        public OSqlSelect()
        {
            _sqlQuery = new SqlQuery();
        }

        internal OSqlSelect(Connection connection)
        {
            _connection = connection;
            _sqlQuery = new SqlQuery();
        }

        #region Select

        public OSqlSelect Select(string projection)
        {
            _sqlQuery.Join(Q.Select, projection);

            return this;
        }

        public OSqlSelect Select(params string[] projections)
        {
            int iteration = 0;
            _sqlQuery.Join(Q.Select);

            foreach (string projection in projections)
            {
                _sqlQuery.Join("", projection);

                iteration++;

                if (iteration < projections.Length)
                {
                    _sqlQuery.Join(Q.Comma);
                }
            }

            return this;
        }

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

        public OSqlSelect Nth(int index)
        {
            _sqlQuery.Join("[" + index + "]");

            return this;
        }

        public OSqlSelect As(string alias)
        {
            _sqlQuery.Join("", Q.As, alias);

            return this;
        }

        #endregion

        #region From

        public OSqlSelect From(string target)
        {
            _sqlQuery.Join("", Q.From, target);

            return this;
        }

        public OSqlSelect From(ORID orid)
        {
            return From(orid.ToString());
        }

        public OSqlSelect From(ODocument document)
        {
            return From(document.ORID);
        }

        public OSqlSelect From<T>()
        {
            return From(typeof(T).Name);
        }

        #endregion

        #region Where with conditions

        public OSqlSelect Where(string field)
        {
            _sqlQuery.Where(field);

            return this;
        }

        public OSqlSelect And(string field)
        {
            _sqlQuery.And(field);

            return this;
        }

        public OSqlSelect Or(string field)
        {
            _sqlQuery.Or(field);

            return this;
        }

        public OSqlSelect Equals<T>(T item)
        {
            _sqlQuery.Equals<T>(item);

            return this;
        }

        public OSqlSelect NotEquals<T>(T item)
        {
            _sqlQuery.NotEquals<T>(item);

            return this;
        }

        public OSqlSelect Lesser<T>(T item)
        {
            _sqlQuery.Lesser<T>(item);

            return this;
        }

        public OSqlSelect LesserEqual<T>(T item)
        {
            _sqlQuery.LesserEqual<T>(item);

            return this;
        }

        public OSqlSelect Greater<T>(T item)
        {
            _sqlQuery.Greater<T>(item);

            return this;
        }

        public OSqlSelect GreaterEqual<T>(T item)
        {
            _sqlQuery.GreaterEqual<T>(item);

            return this;
        }

        public OSqlSelect Like<T>(T item)
        {
            _sqlQuery.Like<T>(item);

            return this;
        }

        public OSqlSelect IsNull()
        {
            _sqlQuery.IsNull();

            return this;
        }

        public OSqlSelect Contains<T>(T item)
        {
            _sqlQuery.Contains<T>(item);

            return this;
        }

        public OSqlSelect Contains<T>(string field, T value)
        {
            _sqlQuery.Contains<T>(field, value);

            return this;
        }

        #endregion

        #region ToList

        public List<T> ToList<T>() where T : class, new()
        {
            List<T> result = new List<T>();
            List<ODocument> documents = ToList("*:0");

            foreach (ODocument document in documents)
            {
                result.Add(document.To<T>());
            }

            return result;
        }

        public List<ODocument> ToList()
        {
            return ToList("*:0");
        }

        public List<ODocument> ToList(string fetchPlan)
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
