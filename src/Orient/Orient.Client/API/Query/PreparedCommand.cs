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
    public class PreparedCommand
    {
        private string _query;
        private Connection _connection;
        private Dictionary<string, object> _parameters;

        public PreparedCommand(string query)
        {
            _query = query;
        }

        internal void SetConnection(Connection connection)
        {
            _connection = connection;
        }

        public OCommandResult Run(params string[] properties)
        {
            if (_connection == null)
                throw new ArgumentNullException("_connection");

            if (_parameters == null)
            {
                _parameters = new Dictionary<string, object>();
            }
            else if (_parameters.Count > 0)
            {
                throw new InvalidOperationException("Can't run prepared query with named parameters");
            }

            for (int i = 0; i < properties.Length; i++)
            {
                _parameters.Add(i.ToString(), properties[i]);
            }

            return RunInternal();
        }

        private OCommandResult RunInternal()
        {
            try
            {
                if (_parameters == null)
                    throw new ArgumentNullException("_parameters");

                var paramsDocument = new ODocument();
                paramsDocument.OClassName = "";
                paramsDocument.SetField(OClient.ProtocolVersion < 22 ? "params" : "parameters", _parameters);

                var serializer = RecordSerializerFactory.GetSerializer(_connection.Database);


                CommandPayloadCommand payload = new CommandPayloadCommand();
                payload.Text = ToString();
                payload.SimpleParams = serializer.Serialize(paramsDocument);

                Command operation = new Command(_connection.Database);
                operation.OperationMode = OperationMode.Synchronous;
                operation.CommandPayload = payload;

                ODocument document = _connection.ExecuteOperation(operation);

                return new OCommandResult(document);
            }
            finally
            {
                _parameters = null;
            }
        }

        public OCommandResult Run()
        {
            return RunInternal();
        }

        public override string ToString()
        {
            return _query;
        }

        public PreparedCommand Set(string key, object value)
        {
            if (_parameters == null)
                _parameters = new Dictionary<string, object>();

            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            _parameters.Add(key, value);

            return this;
        }
    }
}
