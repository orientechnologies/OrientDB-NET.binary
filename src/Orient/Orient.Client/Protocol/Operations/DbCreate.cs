using System.Linq;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class DbCreate : IOperation
    {
        internal string DatabaseName { get; set; }
        internal ODatabaseType DatabaseType { get; set; }
        internal OStorageType StorageType { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Request();
            // standard request fields
            request.DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray((byte)OperationType.DB_CREATE) });
            request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(sessionID) });
            // operation specific fields
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(DatabaseName) });
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(DatabaseType.ToString().ToLower()) });
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(StorageType.ToString().ToLower()) });

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
