using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

namespace Orient.Client.API.Query
{
    public class OClusterQuery
    {
        private List<OCluster> _clusterIds = new List<OCluster>();
        private Connection _connection;

        internal OClusterQuery(Connection _connection)
        {
            this._connection = _connection;
        }

        internal void AddClusterId(OCluster cluster)
        {
            if (!_clusterIds.Contains(cluster))
                _clusterIds.Add(cluster);
        }

        public long Count()
        {
            var operation = new DataClusterCount(_connection.Database);
            operation.Clusters = _clusterIds.Select(c => c.Id).ToList();
            var document = _connection.ExecuteOperation(operation);
            return document.GetField<long>("count");
        }
        public ODocument Range()
        {
            var document = new ODocument();
            foreach (var cluster in _clusterIds)
            {
                var operation = new DataClusterDataRange(_connection.Database);
                operation.ClusterId = cluster.Id;
                var d = _connection.ExecuteOperation(operation);
                if (!string.IsNullOrEmpty(cluster.Name))
                    document.SetField<ODocument>(cluster.Name, d.GetField<ODocument>("Content"));
                else
                    document.SetField<ODocument>(cluster.Id.ToString(), d.GetField<ODocument>("Content"));
            }
            return document;
        }
    }
}
