using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;
using Orient.Client.Protocol.Operations.Command;

namespace Orient.Client
{
    public class OSqlCreateProperty
    {
        private SqlQuery _sqlQuery;
        private Connection _connection;
        private string _propertyName;
        private string _class;
        private OType _type;

        internal OSqlCreateProperty(Connection connection)
        {
            _connection = connection;
            _sqlQuery = new SqlQuery(_connection);
        }
        public OSqlCreateProperty Property(string propertyName, OType type)
        {
            _propertyName = propertyName;
            _type = type;
            _sqlQuery.Property(_propertyName, _type);
            return this;
        }
        public short Run()
        {
            if (string.IsNullOrEmpty(_class))
                throw new OException(OExceptionType.Query, "Class is empty");

            CommandPayloadCommand payload = new CommandPayloadCommand();
            payload.Text = ToString();

            Command operation = new Command(_connection.Database);
            operation.OperationMode = OperationMode.Synchronous;
            operation.CommandPayload = payload;

            OCommandResult result = new OCommandResult(_connection.ExecuteOperation(operation));

            return short.Parse(result.ToDocument().GetField<string>("Content"));
        }

        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.CreateProperty);
        }

        public OSqlCreateProperty Class(string @class)
        {
            _class = @class;
            _sqlQuery.Class(_class);
            return this;
        }

        public OSqlCreateProperty LinkedType(OType type)
        {
            _sqlQuery.LinkedType(type);
            return this;
        }

        public OSqlCreateProperty LinkedClass(string @class)
        {
            _sqlQuery.LinkedClass(@class);
            return this;
        }
    }
}
