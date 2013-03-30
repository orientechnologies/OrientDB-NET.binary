using System.Collections.Generic;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

namespace Orient.Client
{
    public class OSqlSelect
    {
        private Connection _connection;
        private string _sql;

        internal OSqlSelect(Connection connection)
        {
            _connection = connection;
            _sql = "";
        }

        // SELECT [FROM <Target> [LET <Assignment>*](<Projections>]) [<Condition>*](WHERE) [BY <Field>](GROUP) [BY <Fields>* [ASC|DESC](ORDER)*] [<SkipRecords>](SKIP) [<MaxRecords>](LIMIT)
        public OSqlSelect Select(params string[] projections)
        {
            int iteration = 0;
            _sql = string.Join(" ", Q.Select, "");

            foreach (string projection in projections)
            {
                _sql += projection;

                iteration++;

                if (iteration < projections.Length)
                {
                    _sql += string.Join(" ", ",", "");
                }
            }

            return this;
        }

        public OSqlSelect From(string target)
        {
            _sql += string.Join(" ", "", Q.From, target);

            return this;
        }

        public OSqlSelect From<T>()
        {
            _sql += string.Join(" ", "", Q.From, typeof(T).Name);

            return this;
        }

        public List<ORecord> ToList()
        {
            OCommandResult commandResult = Execute(_sql, "*:0");

            return commandResult.ToList();
        }

        private OCommandResult Execute(string sql, string fetchPlan)
        {
            CommandPayload payload = new CommandPayload();
            payload.Type = CommandPayloadType.Sql;
            payload.Text = sql;
            payload.NonTextLimit = -1;
            payload.FetchPlan = fetchPlan;
            payload.SerializedParams = new byte[] { 0 };

            Command operation = new Command();
            operation.OperationMode = OperationMode.Asynchronous;
            operation.ClassType = CommandClassType.Idempotent;
            operation.CommandPayload = payload;

            return new OCommandResult(_connection.ExecuteOperation<Command>(operation));
        }
    }
}
