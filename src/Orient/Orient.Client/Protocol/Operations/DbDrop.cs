using System.Linq;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class DbDrop : BaseOperation
    {
        public DbDrop(ODatabase database)
            :base(database)
        {

        }
        internal string DatabaseName { get; set; }
        internal OStorageType StorageType { get; set; }

        public override Request Request(Request request)
        {
            // standard request fields
            request.AddDataItem((byte)OperationType.DB_DROP);
            request.AddDataItem(request.SessionId);
            // operation specific fields
            request.AddDataItem(DatabaseName);
            if (OClient.ProtocolVersion >= 16) //since 1.5 snapshot but not in 1.5
                request.AddDataItem(StorageType.ToString().ToLower());

            return request;
        }

        public override ODocument Response(Response response)
        {
            // there are no specific response fields processing for this operation

            return null;
        }

    }
}
