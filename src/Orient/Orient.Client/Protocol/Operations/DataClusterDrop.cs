using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orient.Client.Protocol.Operations
{
    internal class DataClusterDrop : BaseOperation
    {
        public short ClusterId { get; set; }

        public DataClusterDrop(ODatabase database)
            : base(database)
        {
                
        }
        public override Request Request(Request request)
        {
            // standard request fields
            request.AddDataItem((byte)OperationType.DATACLUSTER_DROP);
            request.AddDataItem(request.SessionId);

            if (OClient.ProtocolVersion >= 18)
            {
                request.AddDataItem(ClusterId);
            }
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
            var b = reader.ReadByte();
            var removeLocaly = b == 1 ? true : false;
            document.SetField<bool>("remove_localy", removeLocaly);
            return document;
        }
    }
}
