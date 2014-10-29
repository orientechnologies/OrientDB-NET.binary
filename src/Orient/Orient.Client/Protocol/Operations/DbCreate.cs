using System.Linq;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class DbCreate : BaseOperation
    {
        public DbCreate(ODatabase database)
            :base(database)
        {

        }
        internal string DatabaseName { get; set; }
        internal ODatabaseType DatabaseType { get; set; }
        internal OStorageType StorageType { get; set; }

        public override Request Request(Request request)
        {
            // standard request fields
            request.AddDataItem((byte)OperationType.DB_CREATE);
            request.AddDataItem(request.SessionId);
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
