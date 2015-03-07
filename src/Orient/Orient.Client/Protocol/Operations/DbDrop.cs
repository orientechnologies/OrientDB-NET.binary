using System.Linq;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class DbDrop : BaseOperation
    {
        public DbDrop(ODatabase database)
            : base(database)
        {
            _operationType = OperationType.DB_DROP;
        }
        internal string DatabaseName { get; set; }
        internal OStorageType StorageType { get; set; }

        public override Request Request(Request request)
        {
            base.Request(request);

            // operation specific fields
            request.AddDataItem(DatabaseName);
            if (OClient.ProtocolVersion >= 16) //since 1.5 snapshot but not in 1.5
                request.AddDataItem(StorageType.ToString().ToLower());

            return request;
        }

        public override ODocument Response(Response response)
        {
            // there are no specific response fields processing for this operation
            var reader = response.Reader;
            if (response.Connection.ProtocolVersion > 26 && response.Connection.UseTokenBasedSession)
                ReadToken(reader);
            return null;
        }

    }
}
