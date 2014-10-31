using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orient.Client.Protocol.Operations
{
    internal class DataClusterAdd : BaseOperation
    {
        public OClusterType ClusterType { get; set; }

        public string ClusterName { get; set; }

        public DataClusterAdd(ODatabase database)
            : base(database)
        {

        }

        public override ODocument Response(Response response)
        {
            ODocument document = new ODocument();
            if (response == null)
            {
                return document;
            }

            var reader = response.Reader;
            var clusterid = reader.ReadInt16EndianAware();
            document.SetField<short>("clusterid", clusterid);

            return document;
        }

        public override Request Request(Request request)
        {
            request.AddDataItem((byte)OperationType.DATACLUSTER_ADD);
            request.AddDataItem(request.SessionId);

            if (OClient.ProtocolVersion < 24)
                request.AddDataItem(ClusterType.ToString().ToUpper());

            request.AddDataItem(ClusterName);

            if (OClient.ProtocolVersion >= 18)
            {
                request.AddDataItem((short)-1); //clusterid
            }
            return request;
        }
    }
}
