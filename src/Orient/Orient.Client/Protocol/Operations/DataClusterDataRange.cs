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
            _operationType = OperationType.DATACLUSTER_DATARANGE;
        }

        public override Request Request(Request request)
        {
            base.Request(request);

            request.OperationMode = OperationMode.Synchronous;
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
            if (response.Connection.ProtocolVersion > 26 && response.Connection.UseTokenBasedSession)
                ReadToken(reader);

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
