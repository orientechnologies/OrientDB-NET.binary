using System;
using System.Collections.Generic;
using System.Linq;
using Orient.Client.API.Types;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class DbOpen : BaseOperation
    {
        public DbOpen(ODatabase database)
            : base(database)
        {
            _operationType = OperationType.DB_OPEN;
        }

        internal string DatabaseName { get; set; }
        internal ODatabaseType DatabaseType { get; set; }
        internal string UserName { get; set; }
        internal string UserPassword { get; set; }
        internal string ClusterConfig { get { return "null"; } }

        public override Request Request(Request request)
        {
            // standard request fields
            request.AddDataItem((byte)_operationType);
            request.AddDataItem(request.SessionId);
            // operation specific fields
            if (OClient.ProtocolVersion >= 7)
            {
                request.AddDataItem(OClient.DriverName);
                request.AddDataItem(OClient.DriverVersion);
                request.AddDataItem(OClient.ProtocolVersion);
                request.AddDataItem(OClient.ClientID);
            }

            if (OClient.ProtocolVersion > 21)
            {
                request.AddDataItem(OClient.SerializationImpl);
            }

            if (OClient.ProtocolVersion > 26)
            {
                request.AddDataItem((byte)(request.Connection.UseTokenBasedSession ? 1 : 0)); // Use Token Session 0 - false, 1 - true
            }

            request.AddDataItem(DatabaseName);
            if (OClient.ProtocolVersion >= 8)
            {
                request.AddDataItem(DatabaseType.ToString().ToLower());
            }
            request.AddDataItem(UserName);
            request.AddDataItem(UserPassword);

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

            var sessionId = reader.ReadInt32EndianAware();
            document.SetField("SessionId", sessionId);

            if (response.Connection.ProtocolVersion > 26)
            {
                var size = reader.ReadInt32EndianAware();
                var token = reader.ReadBytesRequired(size);
                var t = OToken.Parse(token);
                document.SetField("Token", token);
            }

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

                    byte[] clusterByte = reader.ReadBytes(clusterNameLength);
                    cluster.Name = System.Text.Encoding.UTF8.GetString(clusterByte,0,clusterByte.Length);

                    cluster.Id = reader.ReadInt16EndianAware();

                    if (OClient.ProtocolVersion < 24)
                    {
                        int clusterTypeLength = reader.ReadInt32EndianAware();

                        byte[] clusterTypeByte = reader.ReadBytes(clusterTypeLength);
                        string clusterType = System.Text.Encoding.UTF8.GetString(clusterTypeByte,0,clusterTypeByte.Length);
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
