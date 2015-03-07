using System.Linq;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class DbClose : BaseOperation
    {
        public DbClose(ODatabase database)
            : base(database)
        {
            _operationType = OperationType.DB_CLOSE;
        }
        public override Request Request(Request request)
        {
            base.Request(request);
            
            request.OperationMode = OperationMode.Asynchronous;

            return request;
        }

        public override ODocument Response(Response response)
        {
            // there are no specific response fields which have to be processed for this operation
            var reader = response.Reader;
            if (response.Connection.ProtocolVersion > 26 && response.Connection.UseTokenBasedSession)
                ReadToken(reader);

            return null;
        }

    }
}
