using System.Collections.Generic;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

// syntax:
// SELECT [FROM <Target> 
// [LET <Assignment>*](<Projections>]) 
// [<Condition>*](WHERE) 
// [BY <Field>](GROUP) 
// [BY <Fields>* [ASC|DESC](ORDER)*] 
// [<SkipRecords>](SKIP) 
// [<MaxRecords>](LIMIT)

namespace Orient.Client
{
    public class OSqlSelect
    {
        private SqlQuery _sqlQuery = new SqlQuery();
        private SqlQuery2 _sqlQuery2 = new SqlQuery2();
        private Connection _connection;

        public OSqlSelect()
        {
        }

        internal OSqlSelect(Connection connection)
        {
            _connection = connection;
        }

        #region Select

        /*public OSqlSelect Select(string projection)
        {
            _sqlQuery.Join(Q.Select, projection);

            return this;
        }*/

        public OSqlSelect Select(params string[] projections)
        {
            _sqlQuery2.Select(projections);

            return this;
        }

        public OSqlSelect Also(string projection)
        {
            _sqlQuery2.Also(projection);

            return this;
        }

        /*public OSqlSelect First()
        {
            _sqlQuery.Surround("first");

            return this;
        }*/

        public OSqlSelect Nth(int index)
        {
            _sqlQuery2.Nth(index);

            return this;
        }

        public OSqlSelect As(string alias)
        {
            _sqlQuery2.As(alias);

            return this;
        }

        #endregion

        #region From

        public OSqlSelect From(string target)
        {
            _sqlQuery2.From(target);

            return this;
        }

        public OSqlSelect From(ORID orid)
        {
            _sqlQuery2.From(orid);

            return this;
        }

        public OSqlSelect From(ODocument document)
        {
            if ((document.ORID == null) && string.IsNullOrEmpty(document.OClassName))
            {
                throw new OException(OExceptionType.Query, "Document doesn't contain ORID or OClassName value.");
            }

            _sqlQuery2.From(document);

            return this;
        }

        public OSqlSelect From<T>()
        {
            return From(typeof(T).Name);
        }

        #endregion

        #region Where with conditions

        public OSqlSelect Where(string field)
        {
            _sqlQuery2.Where(field);

            return this;
        }

        public OSqlSelect And(string field)
        {
            _sqlQuery2.And(field);

            return this;
        }

        public OSqlSelect Or(string field)
        {
            _sqlQuery2.Or(field);

            return this;
        }

        public OSqlSelect Equals<T>(T item)
        {
            _sqlQuery2.Equals<T>(item);

            return this;
        }

        public OSqlSelect NotEquals<T>(T item)
        {
            _sqlQuery2.NotEquals<T>(item);

            return this;
        }

        public OSqlSelect Lesser<T>(T item)
        {
            _sqlQuery2.Lesser<T>(item);

            return this;
        }

        public OSqlSelect LesserEqual<T>(T item)
        {
            _sqlQuery2.LesserEqual<T>(item);

            return this;
        }

        public OSqlSelect Greater<T>(T item)
        {
            _sqlQuery2.Greater<T>(item);

            return this;
        }

        public OSqlSelect GreaterEqual<T>(T item)
        {
            _sqlQuery2.GreaterEqual<T>(item);

            return this;
        }

        public OSqlSelect Like<T>(T item)
        {
            _sqlQuery2.Like<T>(item);

            return this;
        }

        public OSqlSelect IsNull()
        {
            _sqlQuery2.IsNull();

            return this;
        }

        public OSqlSelect Contains<T>(T item)
        {
            _sqlQuery2.Contains<T>(item);

            return this;
        }

        public OSqlSelect Contains<T>(string field, T value)
        {
            _sqlQuery2.Contains<T>(field, value);

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
            payload.Text = ToString();
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
            return _sqlQuery2.ToString(QueryType.Select);
        }
    }
}
