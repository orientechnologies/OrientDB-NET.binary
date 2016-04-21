using System.IO;
using System.Linq;
using System.Text;
using Orient.Client.API.Types;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class Connect : BaseOperation
    {
        public Connect(ODatabase database)
            : base(database)
        {
            _operationType = OperationType.CONNECT;
        }

        internal string UserName { get; set; }
        internal string UserPassword { get; set; }

        public override Request Request(Request request)
        {
            // standard request fields
            request.AddDataItem((byte)_operationType);
            request.AddDataItem(request.SessionId);

            // operation specific fields
            if (OClient.ProtocolVersion > 7)
            {
                request.AddDataItem(OClient.DriverName);
                request.AddDataItem(OClient.DriverVersion);
                request.AddDataItem(OClient.ProtocolVersion);
                request.AddDataItem(OClient.ClientID);
            }
            if (OClient.ProtocolVersion > 21)
            {
                request.AddDataItem(OClient.SerializationImpl);
            }

            if (OClient.ProtocolVersion > 26)
            {
                request.AddDataItem((byte)(request.Connection.UseTokenBasedSession ? 1 : 0)); // Use Token Session 0 - false, 1 - true
            }
            request.AddDataItem(UserName);
            request.AddDataItem(UserPassword);

            return request;
        }

        public override ODocument Response(Response response)
        {
            ODocument document = new ODocument();

            if (response == null)
            {
                return document;
            }
            var reader = response.Reader;

            var sessionId = reader.ReadInt32EndianAware();
            document.SetField("SessionId", sessionId);

            if (response.Connection.ProtocolVersion > 26)
            {
                var size = reader.ReadInt32EndianAware();
                var token = reader.ReadBytesRequired(size);
                var t = OToken.Parse(token);
                document.SetField("Token", token);
            }

            return document;
        }

    }
}
