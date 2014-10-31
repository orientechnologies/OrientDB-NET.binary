using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

namespace Orient.Client.API.Query
{
    public class OSqlDeleteCluster
    {
        private Connection _connection;
        private short _clusterid;

        public OSqlDeleteCluster()
        {

        }

        internal OSqlDeleteCluster(Connection connection, short clusterid)
        {
            _connection = connection;
            _clusterid = clusterid;
        }
        public bool Run()
        {
            bool result = false;
            var operation = new DataClusterDrop(_connection.Database);
            operation.ClusterId = _clusterid;
            var document = _connection.ExecuteOperation(operation);
            result = document.GetField<bool>("remove_localy");
            if (result)
                _connection.Database.RemoveCluster(_clusterid);
            return result;
        }
    }
}
