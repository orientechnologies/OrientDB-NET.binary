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

        public OSqlSelect From<T>()
        {
            return From(typeof(T).Name);
        }

        public OSqlSelect From(string target)
        {
            _sqlQuery.Join("", Q.From, target);

            return this;
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
