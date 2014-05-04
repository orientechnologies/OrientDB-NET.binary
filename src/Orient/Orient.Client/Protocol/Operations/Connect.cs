using System.Linq;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class Connect : IOperation
    {
        internal string UserName { get; set; }
        internal string UserPassword { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Request();

            // standard request fields
            request.DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray((byte)OperationType.CONNECT) });
            request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(sessionID) });
            // operation specific fields
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(OClient.DriverName) });
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(OClient.DriverVersion) });
            request.DataItems.Add(new RequestDataItem() { Type = "short", Data = BinarySerializer.ToArray(OClient.ProtocolVersion) });
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(OClient.ClientID) });
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(UserName) });
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(UserPassword) });

            return request;
        }

        public ODocument Response(Response response)
        {
            ODocument document = new ODocument();

            if (response == null)
            {
                return document;
            }

            var reader = response.Reader;

            // operation specific fields
            document.SetField("SessionId", reader.ReadInt32EndianAware());

            return document;
        }
    }
}
