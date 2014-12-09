using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orient.Client.Protocol.Operations
{
    internal class DataClusterDataRange : BaseOperation
    {
        public short ClusterId { get; set; }

        public DataClusterDataRange(ODatabase database)
            : base(database)
        {

        }

        public override Request Request(Request request)
        {
            request.OperationMode = OperationMode.Synchronous;

            // standard request fields
            request.AddDataItem((byte)OperationType.DATACLUSTER_DATARANGE);
            request.AddDataItem(request.SessionId);
            request.AddDataItem(ClusterId);

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
            var begin = reader.ReadInt64EndianAware();
            var end = reader.ReadInt64EndianAware();
            var embededDoc = new ODocument();
            embededDoc.SetField("begin", begin);
            embededDoc.SetField("end", end);
            document.SetField<ODocument>("Content", embededDoc);

            return document;
        }
    }
}
