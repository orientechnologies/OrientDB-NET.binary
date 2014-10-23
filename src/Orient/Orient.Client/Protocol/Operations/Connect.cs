using System.Linq;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class Connect : IOperation
    {
        internal string UserName { get; set; }
        internal string UserPassword { get; set; }

        public Request Request(Request request)
        {

            // standard request fields
            request.AddDataItem((byte)OperationType.CONNECT);
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
            request.AddDataItem(UserName);
            request.AddDataItem(UserPassword);

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
