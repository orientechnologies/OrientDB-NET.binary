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
            : base(database)
        {
            _operationType = OperationType.DATACLUSTER_COUNT;
        }
        public override Request Request(Request request)
        {
            base.Request(request);

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
                request.AddDataItem((byte)(CountTombStones ? 1 : 0));

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

            var size = reader.ReadInt64EndianAware();
            document.SetField<long>("count", size);

            return document;
        }
    }
}
