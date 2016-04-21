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
            _operationType = OperationType.DATACLUSTER_ADD;
        }

        public override Request Request(Request request)
        {
            base.Request(request);

            if (OClient.ProtocolVersion < 24)
                request.AddDataItem(ClusterType.ToString().ToUpper());

            request.AddDataItem(ClusterName);

            if (OClient.ProtocolVersion < 24)
            {
                if (OClient.ProtocolVersion >= 10 || ClusterType == OClusterType.Physical)
                    request.AddDataItem("");

                if (OClient.ProtocolVersion >= 10)
                    request.AddDataItem("default");
                else
                    request.AddDataItem((short)-1);
            }
            if (OClient.ProtocolVersion >= 18)
            {
                request.AddDataItem((short)-1); //clusterid
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
            if (response.Connection.ProtocolVersion > 26 && response.Connection.UseTokenBasedSession)
                ReadToken(reader);

            var clusterid = reader.ReadInt16EndianAware();
            document.SetField<short>("clusterid", clusterid);

            return document;
        }
    }
}
