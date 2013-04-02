using System.Collections.Generic;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

namespace Orient.Client
{
    public class OSqlCreate
    {
        private Connection _connection;

        internal OSqlCreate(Connection connection)
        {
            _connection = connection;
        }

        #region Class

        public OSqlCreateClass Class(string className)
        {
            return new OSqlCreateClass(_connection).Class(className);
        }

        public OSqlCreateClass Class<T>()
        {
            return Class(typeof(T).Name);
        }

        #endregion

        #region Cluster

        public OSqlCreateCluster Cluster(string clusterName, OClusterType clusterType)
        {
            return new OSqlCreateCluster(_connection).Cluster(clusterName, clusterType);
        }

        public OSqlCreateCluster Cluster<T>(OClusterType clusterType)
        {
            return Cluster(typeof(T).Name, clusterType);
        }

        #endregion

        #region Edge

        public OSqlCreateEdge Edge(string className)
        {
            return new OSqlCreateEdge(_connection).Edge(className);
        }

        public OSqlCreateEdge Edge<T>()
        {
            return Edge(typeof(T).Name);
        }

        #endregion

        #region Vertex

        public OSqlCreateVertex Vertex(string className)
        {
            return new OSqlCreateVertex(_connection).Vertex(className);
        }

        public OSqlCreateVertex Vertex<T>()
        {
            return Vertex(typeof(T).Name);
        }

        #endregion
    }
}
