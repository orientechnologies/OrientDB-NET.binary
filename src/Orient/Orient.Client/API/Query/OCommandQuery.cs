using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;
using Orient.Client.Protocol.Operations.Command;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.API.Query
{
    public class OCommandQuery
    {
        private Connection _connection;
        private CommandPayloadBase _payload;
        private Dictionary<string, object> _simpleParams;

        internal OCommandQuery(Connection connection, CommandPayloadBase payload)
        {
            _connection = connection;
            _payload = payload;
        }

        public OCommandResult Run()
        {
            if (_simpleParams != null)
            {
                var paramsDocument = new ODocument();
                paramsDocument.OClassName = "";
                paramsDocument.SetField(OClient.ProtocolVersion < 22 ? "params" : "parameters", _simpleParams);
                ((CommandPayloadCommand)_payload).SimpleParams = RecordSerializerFactory.GetSerializer(_connection.Database).Serialize(paramsDocument);
            }

            Command operation = new Command(_connection.Database);
            operation.OperationMode = OperationMode.Synchronous;
            operation.CommandPayload = _payload;

            ODocument document = _connection.ExecuteOperation(operation);
            return new OCommandResult(document);
        }

        public OCommandQuery Set(string parameter, object value)
        {
            if (!(_payload is CommandPayloadCommand))
                throw new OException(OExceptionType.Query, "A command not support simple parameters");

            if (_simpleParams == null)
                _simpleParams = new Dictionary<string, object>();

            _simpleParams.Add(parameter, value);

            return this;
        }
    }
}
