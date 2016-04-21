using Orient.Client.API.Query;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

namespace Orient.Client
{
    public class OSqlDelete
    {
        private Connection _connection;

        internal OSqlDelete(Connection connection)
        {
            _connection = connection;
        }

        #region Document

        public OSqlDeleteDocument Document()
        {
            return new OSqlDeleteDocument(_connection);
        }

        public OSqlDeleteDocument Document(string className)
        {
            return new OSqlDeleteDocument(_connection)
                .Class(className);
        }

        public OSqlDeleteDocument Document<T>()
        {
            return new OSqlDeleteDocument(_connection)
                .Class<T>();
        }

        public OSqlDeleteDocument Document<T>(T obj)
        {
            return new OSqlDeleteDocument(_connection)
                .Delete(obj);
        }

        #endregion

        #region Vertex

        public OSqlDeleteVertex Vertex()
        {
            return new OSqlDeleteVertex(_connection);
        }

        public OSqlDeleteVertex Vertex(string className)
        {
            return new OSqlDeleteVertex(_connection)
                .Class(className);
        }

        public OSqlDeleteVertex Vertex<T>()
        {
            return new OSqlDeleteVertex(_connection)
                .Class<T>();
        }

        public OSqlDeleteVertex Vertex<T>(T obj)
        {
            return new OSqlDeleteVertex(_connection)
                .Delete(obj);
        }

        #endregion

        #region Edge

        public OSqlDeleteEdge Edge()
        {
            return new OSqlDeleteEdge(_connection);
        }

        public OSqlDeleteEdge Edge(string className)
        {
            return new OSqlDeleteEdge(_connection)
                .Class(className);
        }

        public OSqlDeleteEdge Edge<T>()
        {
            return new OSqlDeleteEdge(_connection)
                .Class<T>();
        }

        public OSqlDeleteEdge Edge<T>(T obj)
        {
            return new OSqlDeleteEdge(_connection)
                .Delete(obj);
        }

        #endregion

        #region Cluster

        public OSqlDeleteCluster Cluster(short clusterid)
        {
            return new OSqlDeleteCluster(_connection, clusterid);
        }

        public OSqlDeleteCluster Cluster(string clusterName)
        {
            var clusterid = _connection.Database.GetClusterIdFor(clusterName);
            return Cluster(clusterid);
        }

        public OSqlDeleteCluster Cluster<T>()
        {
            return Cluster(typeof(T).Name);
        }

        #endregion
    }
}
