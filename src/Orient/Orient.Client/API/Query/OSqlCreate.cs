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
        public OSqlCreateProperty Property(string propertyName,OType type)
        {
            return new OSqlCreateProperty(_connection)
                .Property(propertyName,type);
        }
        public OSqlCreateClass Class<T>()
        {
            return new OSqlCreateClass(_connection).Class<T>();
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

        #region Document

        public OSqlCreateDocument Document(string className)
        {
            return new OSqlCreateDocument(_connection)
                .Document(className);
        }

        public OSqlCreateDocument Document<T>()
        {
            return Document(typeof(T).Name);
        }

        public OSqlCreateDocument Document<T>(T obj)
        {
            return new OSqlCreateDocument(_connection)
                .Document(obj);
        }

        #endregion

        #region Vertex

        public OSqlCreateVertex Vertex(string className)
        {
            return new OCreateVertexRecord(_connection)
                .Vertex(className);
        }

        public OSqlCreateVertex Vertex<T>()
        {
            return Vertex(typeof(T).Name);
        }

        public OSqlCreateVertex Vertex<T>(T obj)
        {
            return new OCreateVertexRecord(_connection)
                .Vertex(obj);
        }

        #endregion

        #region Edge

        public OSqlCreateEdge Edge(string className)
        {
            return new OSqlCreateEdgeViaSql(_connection)
                .Edge(className);
        }

        public OSqlCreateEdge Edge<T>()
        {
            return Edge(typeof(T).Name);
        }

        public OSqlCreateEdge Edge<T>(T obj)
        {
            return new OSqlCreateEdgeViaSql(_connection)
                .Edge(obj);
        }

        #endregion
    }
}
