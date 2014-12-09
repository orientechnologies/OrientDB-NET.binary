using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class DBCountRecords : BaseOperation
    {
        public DBCountRecords(ODatabase database)
            : base(database)
        {

        }
        public override Request Request(Request request)
        {
            // standard request fields
            request.AddDataItem((byte)OperationType.DB_COUNTRECORDS);
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
            var size = reader.ReadInt64EndianAware();
            document.SetField<long>("count", size);

            return document;
        }
    }
}
