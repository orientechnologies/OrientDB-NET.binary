using System.Linq;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class DbClose : IOperation
    {
        public Request Request(Request request)
        {
            request.OperationMode = OperationMode.Asynchronous;

            // standard request fields
            request.AddDataItem((byte)OperationType.DB_CLOSE);
            request.AddDataItem(request.SessionId);

            return request;
        }

        public ODocument Response(Response response)
        {
            // there are no specific response fields which have to be processed for this operation

            return null;
        }
    }
}
