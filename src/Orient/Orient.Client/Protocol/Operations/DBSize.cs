using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class DBSize : BaseOperation
    {
        public DBSize(ODatabase database)
            : base(database)
        {

        }
        public override Request Request(Request request)
        {
            // standard request fields
            request.AddDataItem((byte)OperationType.DB_SIZE);
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
            document.SetField<long>("size", size);

            return document;
        }
    }
}
