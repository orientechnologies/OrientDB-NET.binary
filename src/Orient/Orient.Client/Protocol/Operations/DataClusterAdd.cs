using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orient.Client.Protocol.Operations
{
    internal class DataClusterAdd : IOperation
    {
        public OClusterType ClusterType { get; set; }

        public string ClusterName { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Request();
            request.OperationMode = OperationMode.Synchronous;

            // standard request fields
            request.AddDataItem((byte)OperationType.DATACLUSTER_ADD);
            request.AddDataItem(sessionID);

            if (OClient.ProtocolVersion < 24)
                request.AddDataItem(ClusterType.ToString().ToUpper());

            request.AddDataItem(ClusterName);

            if (OClient.ProtocolVersion >= 18)
            {
                request.AddDataItem((short)-1); //clusterid
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
            var clusterid = reader.ReadInt16EndianAware();
            document.SetField<short>("clusterid", clusterid);

            return document;
        }

    }
}
