using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orient.Client.Protocol.Operations
{
    internal class DataClusterDataRange : IOperation
    {
        public short ClusterId { get; set; }

        public DataClusterDataRange()
        {

        }

        public Request Request(int sessionID)
        {
            Request request = new Request();
            request.OperationMode = OperationMode.Synchronous;

            // standard request fields
            request.AddDataItem((byte)OperationType.DATACLUSTER_DATARANGE);
            request.AddDataItem(sessionID);
            request.AddDataItem(ClusterId);

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
