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

        public OSqlCreateClass Class<T>()
        {
            return Class(typeof(T).Name);
        }

        public OSqlCreateClass Class(string className)
        {
            return new OSqlCreateClass(_connection, className);
        }

        #endregion

        #region Cluster

        public OSqlCreateCluster Cluster<T>(OClusterType clusterType)
        {
            return Cluster(typeof(T).Name, clusterType);
        }

        public OSqlCreateCluster Cluster(string clusterName, OClusterType clusterType)
        {
            return new OSqlCreateCluster(_connection, clusterName, clusterType);
        }

        #endregion

        #region Edge

        public OSqlCreateEdge Edge<T>()
        {
            return Edge(typeof(T).Name);
        }

        public OSqlCreateEdge Edge(string className)
        {
            return new OSqlCreateEdge(_connection, className);
        }

        #endregion

        #region Vertex

        public OSqlCreateVertex Vertex<T>()
        {
            return Vertex(typeof(T).Name);
        }

        public OSqlCreateVertex Vertex(string className)
        {
            return new OSqlCreateVertex(_connection, className);
        }

        /*public T Vertex<T>(T obj) where T: class, new()
        {
            return Vertex(typeof(T).Name, null, ODataObject.MapData(obj)).To<T>();
        }

        public T Vertex<T>(string cluster, T obj) where T : class, new()
        {
            return Vertex(typeof(T).Name, cluster, ODataObject.MapData(obj)).To<T>();
        }*/

        #endregion
    }
}
