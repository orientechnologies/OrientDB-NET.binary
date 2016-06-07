using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.API.Query.Interfaces;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

namespace Orient.Client.API.Query
{
    public class ODataClasterAdd : IOCreateCluster
    {
        private Connection _connection;
        public string ClusterName { get; set; }
        public OClusterType ClusterType { get; set; }

        public ODataClasterAdd()
        {

        }

        internal ODataClasterAdd(Connection connection)
        {
            _connection = connection;
        }

        public IOCreateCluster Cluster(string clusterName, OClusterType clusterType)
        {
            ClusterName = clusterName;
            ClusterType = clusterType;
            return this;
        }

        public IOCreateCluster Cluster<T>(OClusterType clusterType)
        {
            return Cluster(typeof(T).Name, clusterType);
        }

        public short Run()
        {
            var operation = new DataClusterAdd(_connection.Database);
            operation.ClusterType = ClusterType;
            operation.ClusterName = ClusterName;
            var document = _connection.ExecuteOperation(operation);
            var clusterid = document.GetField<short>("clusterid");
            if (clusterid != -1)
                _connection.Database.AddCluster(new OCluster { Name = ClusterName, Id = clusterid, Type = ClusterType });
            return clusterid;
        }
    }
}
