using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orient.Client.Protocol.Operations
{
    internal class DataClusterDrop : IOperation
    {
        public short ClusterId { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Request();
            request.OperationMode = OperationMode.Synchronous;

            // standard request fields
            request.AddDataItem((byte)OperationType.DATACLUSTER_DROP);
            request.AddDataItem(sessionID);

            if (OClient.ProtocolVersion >= 18)
            {
                request.AddDataItem(ClusterId);
            }
            return request;
        }

        public ODocument Response(Response response)
        {
            ODocument document = new ODocument();
            if (response == null)
            {
                return document;
            }

            var reader = response.Reader;
            var removeLocaly = reader.ReadByte() == 1 ? true : false;
            document.SetField<bool>("remove_localy", removeLocaly);
            return document;
        }
    }
}
