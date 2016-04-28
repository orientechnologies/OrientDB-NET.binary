using System.Linq;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class DbExist : BaseOperation
    {
        public DbExist(ODatabase database)
            : base(database)
        {
            _operationType = OperationType.DB_EXIST;
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
            ODocument document = new ODocument();

            if (response == null)
            {
                return document;
            }

            var reader = response.Reader;
            if (response.Connection.ProtocolVersion > 26 && response.Connection.UseTokenBasedSession)
                ReadToken(reader);

            // operation specific fields
            byte existByte = reader.ReadByte();

            if (existByte == 0)
            {
                document.SetField("Exists", false);
            }
            else
            {
                document.SetField("Exists", true);
            }

            return document;
        }

    }
}
