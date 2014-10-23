using System.Linq;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class DbCreate : IOperation
    {
        internal string DatabaseName { get; set; }
        internal ODatabaseType DatabaseType { get; set; }
        internal OStorageType StorageType { get; set; }

        public Request Request(Request request)
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

        public ODocument Response(Response response)
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
