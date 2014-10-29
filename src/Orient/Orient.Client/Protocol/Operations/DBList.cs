using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class DBList : BaseOperation
    {
        public DBList(ODatabase database)
            : base(database)
        {

        }
        public override Request Request(Request request)
        {
            request.AddDataItem((byte)OperationType.DB_LIST);
            request.AddDataItem(request.SessionId);
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
            int recordLength = reader.ReadInt32EndianAware();
            byte[] rawRecord = reader.ReadBytes(recordLength);
            document = Serializer.Deserialize(rawRecord, document);
            return document;
        }
    }
}
