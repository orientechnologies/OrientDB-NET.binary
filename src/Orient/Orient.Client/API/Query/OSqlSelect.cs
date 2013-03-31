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

        public OSqlSelect As(string alias)
        {
            _sqlQuery.Join("", Q.As, alias);

            return this;
        }

        public OSqlSelect And(string projection)
        {
            _sqlQuery.Join(Q.Comma, projection);

            return this;
        }

        public OSqlSelect From<T>()
        {
            return From(typeof(T).Name);
        }

        public OSqlSelect From(ORID orid)
        {
            _sqlQuery.Join("", Q.From, orid.ToString());

            return this;
        }

        public OSqlSelect From(string target)
        {
            _sqlQuery.Join("", Q.From, target);

            return this;
        }

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
            payload.Text = _sqlQuery.Value;
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
    }
}
