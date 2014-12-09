using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orient.Client.Protocol.Operations
{
    internal class DataClusterCount : BaseOperation
    {
        public List<short> Clusters { get; set; }
        public Boolean CountTombStones = false;

        public DataClusterCount(ODatabase database)
            :base(database)
        {

        }
        public override Request Request(Request request)
        {
            // standard request fields
            request.AddDataItem((byte)OperationType.DATACLUSTER_COUNT);
            request.AddDataItem(request.SessionId);

            request.AddDataItem((short)Clusters.Count);
            foreach (var item in Clusters)
            {
                request.AddDataItem(item);
            }

            /*
             * count-tombstones the flag which indicates whether deleted records should be taken in account. 
             * It is applicable for autosharded storage only, otherwise it is ignored.
             */
            if (OClient.ProtocolVersion >= 13)
                request.AddDataItem((CountTombStones) ? (byte)1 : (byte)0);

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
