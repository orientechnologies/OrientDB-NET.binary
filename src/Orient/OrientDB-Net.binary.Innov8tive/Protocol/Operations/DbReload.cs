using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    class DbReload : BaseOperation
    {
        public DbReload(ODatabase database)
            : base(database)
        {
            _operationType = OperationType.DB_RELOAD;
        }
        public override Request Request(Request request)
        {
            base.Request(request);
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

            short clusterCount = reader.ReadInt16EndianAware();
            document.SetField("ClusterCount", clusterCount);

            if (clusterCount > 0)
            {
                List<OCluster> clusters = new List<OCluster>();

                for (int i = 1; i <= clusterCount; i++)
                {
                    OCluster cluster = new OCluster();

                    int clusterNameLength = reader.ReadInt32EndianAware();

                    byte[] rawBytes = reader.ReadBytes(clusterNameLength);
                    cluster.Name = System.Text.Encoding.UTF8.GetString(rawBytes,0,rawBytes.Length);

                    cluster.Id = reader.ReadInt16EndianAware();

                    if (OClient.ProtocolVersion < 24)
                    {
                        int clusterTypeLength = reader.ReadInt32EndianAware();

                        byte[] clusterBytes = reader.ReadBytes(clusterTypeLength);
                        string clusterType = System.Text.Encoding.UTF8.GetString(clusterBytes,0,clusterBytes.Length);
                        //cluster.Type = (OClusterType)Enum.Parse(typeof(OClusterType), clusterType, true);
                        if (OClient.ProtocolVersion >= 12)
                            cluster.DataSegmentID = reader.ReadInt16EndianAware();
                        else
                            cluster.DataSegmentID = 0;
                    }

                    clusters.Add(cluster);
                }

                document.SetField("Clusters", clusters);
            }

            return document;
        }
    }
}
