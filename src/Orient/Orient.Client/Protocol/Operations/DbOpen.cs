using System;
using System.Collections.Generic;
using System.Linq;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class DbOpen : IOperation
    {
        internal string DatabaseName { get; set; }
        internal ODatabaseType DatabaseType { get; set; }
        internal string UserName { get; set; }
        internal string UserPassword { get; set; }
        internal string ClusterConfig { get { return "null"; } }

        public Request Request(int sessionID)
        {
            Request request = new Request();
            // standard request fields
            request.DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray((byte)OperationType.DB_OPEN) });
            request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(sessionID) });
            // operation specific fields
            if (OClient.ProtocolVersion >= 7)
            {
                request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(OClient.DriverName) });
                request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(OClient.DriverVersion) });
                request.DataItems.Add(new RequestDataItem() { Type = "short", Data = BinarySerializer.ToArray(OClient.ProtocolVersion) });
                request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(OClient.ClientID) });
            }
            if (OClient.ProtocolVersion > 21)
            {
                request.DataItems.Add(new RequestDataItem { Type = "string", Data = BinarySerializer.ToArray(OClient.SerializationImpl) });
            }

            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(DatabaseName) });
            if (OClient.ProtocolVersion >= 8)
            {
                request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(DatabaseType.ToString().ToLower()) });
            }
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(UserName) });
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(UserPassword) });

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

            // operation specific fields
            document.SetField("SessionId", reader.ReadInt32EndianAware());
            int clusterCount = -1;

            if (OClient.ProtocolVersion >= 7)
                clusterCount = (int)reader.ReadInt16EndianAware();
            else
                clusterCount = reader.ReadInt32EndianAware();

            document.SetField("ClusterCount", clusterCount);

            if (clusterCount > 0)
            {
                List<OCluster> clusters = new List<OCluster>();

                for (int i = 1; i <= clusterCount; i++)
                {
                    OCluster cluster = new OCluster();

                    int clusterNameLength = reader.ReadInt32EndianAware();

                    cluster.Name = System.Text.Encoding.Default.GetString(reader.ReadBytes(clusterNameLength));

                    cluster.Id = reader.ReadInt16EndianAware();

                    if (OClient.ProtocolVersion < 24)
                    {
                        int clusterTypeLength = reader.ReadInt32EndianAware();

                        string clusterType = System.Text.Encoding.Default.GetString(reader.ReadBytes(clusterTypeLength));
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

            int clusterConfigLength = reader.ReadInt32EndianAware();

            byte[] clusterConfig = null;

            if (clusterConfigLength > 0)
            {
                clusterConfig = reader.ReadBytes(clusterConfigLength);
            }

            document.SetField("ClusterConfig", clusterConfig);

            string release = reader.ReadInt32PrefixedString();
            document.SetField("OrientdbRelease", release);

            return document;
        }
    }
}
