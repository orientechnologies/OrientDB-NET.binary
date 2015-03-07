using System.Linq;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class DbCreate : BaseOperation
    {
        public DbCreate(ODatabase database)
            : base(database)
        {
            _operationType = OperationType.DB_CREATE;
        }
        internal string DatabaseName { get; set; }
        internal ODatabaseType DatabaseType { get; set; }
        internal OStorageType StorageType { get; set; }

        public override Request Request(Request request)
        {
            base.Request(request);

            // operation specific fields
            request.AddDataItem(DatabaseName);
            request.AddDataItem(DatabaseType.ToString().ToLower());
            request.AddDataItem(StorageType.ToString().ToLower());

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
            if (response.Connection.ProtocolVersion > 26 && response.Connection.UseTokenBasedSession)
                ReadToken(reader);

            if (response.Status == ResponseStatus.OK)
            {
                document.SetField("IsCreated", true);
            }
            else
            {
                document.SetField("IsCreated", true);
            }

            return document;
        }

    }
}
