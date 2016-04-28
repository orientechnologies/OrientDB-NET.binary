using Orient.Client.API.Query;
using Orient.Client.API.Query.Interfaces;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

namespace Orient.Client
{
    public class OCreate
    {
        private Connection _connection;

        internal OCreate(Connection connection)
        {
            _connection = connection;
        }

        #region Class

        public OSqlCreateClass Class(string className)
        {
            return new OSqlCreateClass(_connection).Class(className);
        }
        public OSqlCreateProperty Property(string propertyName, OType type)
        {
            return new OSqlCreateProperty(_connection)
                .Property(propertyName, type);
        }
        public OSqlCreateClass Class<T>()
        {
            return new OSqlCreateClass(_connection).Class<T>();
        }

        public OSqlCreateClass Class<T>(string className)
        {
            return new OSqlCreateClass(_connection).Class<T>(className);
        }

        #endregion

        #region Cluster

        public IOCreateCluster Cluster(string clusterName, OClusterType clusterType)
        {
            return new ODataClasterAdd(_connection).Cluster(clusterName, clusterType);
        }

        public IOCreateCluster Cluster<T>(OClusterType clusterType)
        {
            return Cluster(typeof(T).Name, clusterType);
        }

        #endregion

        #region Document

        public IOCreateDocument Document(string className)
        {
            return new ORecordCreateDocument(_connection).Document(className);
        }

        public IOCreateDocument Document<T>()
        {
            return Document(typeof(T).Name);
        }

        public IOCreateDocument Document<T>(T obj)
        {
            return new ORecordCreateDocument(_connection).Document(obj);
        }

        #endregion

        #region Vertex

        public IOCreateVertex Vertex(string className)
        {
            return new ORecordCreateVertex(_connection)
                .Vertex(className);
        }

        public IOCreateVertex Vertex<T>()
        {
            return Vertex(typeof(T).Name);
        }

        public IOCreateVertex Vertex<T>(T obj)
        {
            return new ORecordCreateVertex(_connection)
                .Vertex(obj);
        }

        #endregion

        #region Edge

        public IOCreateEdge Edge(string className)
        {
            return new OSqlCreateEdge(_connection)
                .Edge(className);
        }

        public IOCreateEdge Edge<T>()
        {
            return Edge(typeof(T).Name);
        }

        public IOCreateEdge Edge<T>(T obj)
        {
            return new OSqlCreateEdge(_connection)
                .Edge(obj);
        }

        #endregion
    }
}
