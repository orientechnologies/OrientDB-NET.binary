using System.Linq;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class DbClose : IOperation
    {
        public Request Request(int sessionID)
        {
            Request request = new Request();
            request.OperationMode = OperationMode.Asynchronous;

            // standard request fields
            request.DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray((byte)OperationType.DB_CLOSE) });
            request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(sessionID) });

            return request;
        }

        public ResponseDataObject Response(Response response)
        {
            // there are no specific response fields which have to be processed for this operation

            return null;
        }
    }
}
