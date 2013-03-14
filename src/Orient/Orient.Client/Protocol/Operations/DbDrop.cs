using System.Linq;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class DbDrop : IOperation
    {
        internal string DatabaseName { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Request();
            // standard request fields
            request.DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray((byte)OperationType.DB_DROP) });
            request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(sessionID) });
            // operation specific fields
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(DatabaseName) });

            return request;
        }

        public DataObject Response(Response response)
        {
            // there are no specific response fields processing for this operation

            return null;
        }
    }
}
