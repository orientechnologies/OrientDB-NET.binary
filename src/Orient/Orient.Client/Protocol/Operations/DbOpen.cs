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
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(OClient.DriverName) });
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(OClient.DriverVersion) });
            request.DataItems.Add(new RequestDataItem() { Type = "short", Data = BinarySerializer.ToArray(OClient.ProtocolVersion) });
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(OClient.ClientID) });
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(DatabaseName) });
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(DatabaseType.ToString().ToLower()) });
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(UserName) });
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(UserPassword) });

            return request;
        }

        public ODocument Response(Response response)
        {
            // start from this position since standard fields (status, session ID) has been already parsed
            int offset = 5;
            ODocument document = new ODocument();

            if (response == null)
            {
                return document;
            }

            // operation specific fields
            document.SetField("SessionId", BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray()));
            offset += 4;
            short clusterCount = BinarySerializer.ToShort(response.Data.Skip(offset).Take(2).ToArray());
            document.SetField("ClusterCount", clusterCount);
            offset += 2;

            if (clusterCount > 0)
            {
                List<OCluster> clusters = new List<OCluster>();

                for (int i = 1; i <= clusterCount; i++)
                {
                    OCluster cluster = new OCluster();

                    int clusterNameLength = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                    offset += 4;

                    cluster.Name = BinarySerializer.ToString(response.Data.Skip(offset).Take(clusterNameLength).ToArray());
                    offset += clusterNameLength;

                    cluster.Id = BinarySerializer.ToShort(response.Data.Skip(offset).Take(2).ToArray());
                    offset += 2;

                    int clusterTypeLength = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                    offset += 4;

                    string clusterName = BinarySerializer.ToString(response.Data.Skip(offset).Take(clusterTypeLength).ToArray());
                    cluster.Type = (OClusterType)Enum.Parse(typeof(OClusterType), clusterName, true);
                    offset += clusterTypeLength;

                    cluster.DataSegmentID = BinarySerializer.ToShort(response.Data.Skip(offset).Take(2).ToArray());
                    offset += 2;

                    clusters.Add(cluster);
                }

                document.SetField("Clusters", clusters);
            }

            int clusterConfigLength = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray());
            offset += 4;

            document.SetField("ClusterConfig", response.Data.Skip(offset).Take(clusterConfigLength).ToArray());
            offset += clusterConfigLength;

            return document;
        }
    }
}
