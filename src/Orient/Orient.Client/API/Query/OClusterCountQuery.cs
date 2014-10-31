using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

namespace Orient.Client.API.Query
{
    public class OClusterCountQuery
    {
        private List<short> _clusterIds = new List<short>();
        private Connection _connection;

        internal OClusterCountQuery(Connection _connection)
        {
            this._connection = _connection;
        }

        internal void AddClusterId(short clusterid)
        {
            if (!_clusterIds.Contains(clusterid))
                _clusterIds.Add(clusterid);
        }

        public long Count()
        {
            var operation = new DataClusterCount();
            operation.Clusters = _clusterIds;
            var document = _connection.ExecuteOperation(operation);
            return document.GetField<long>("count");
        }
    }
}
