using System.Collections.Generic;
using System.Linq;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

// syntax: 
// CREATE VERTEX [<class>] 
// [CLUSTER <cluster>] 
// [SET <field> = <expression>[,]*]

namespace Orient.Client
{
    public class OSqlCreateVertexDirect
    {
        private Connection _connection;
        private ODocument _document;

        public OSqlCreateVertexDirect()
        {
        }

        internal OSqlCreateVertexDirect(Connection connection)
        {
            _connection = connection;
        }

        #region Vertex

        public OSqlCreateVertexDirect Vertex(string className)
        {
            if (_document == null)
                _document = new ODocument();

            _document.OClassName = className;

            return this;
        }

        public OSqlCreateVertexDirect Vertex<T>(T obj)
        {

            if (obj is ODocument)
            {
                _document = obj as ODocument;
            }
            else
            {
                _document = ODocument.ToDocument(obj);
            }

            if (string.IsNullOrEmpty(_document.OClassName))
            {
                throw new OException(OExceptionType.Query, "Document doesn't contain OClassName value.");
            }

            return this;
        }

        public OSqlCreateVertexDirect Vertex<T>()
        {
            return Vertex(typeof(T).Name);
        }

        #endregion

        #region Cluster

        public OSqlCreateVertexDirect Cluster(string clusterName)
        {
            if (_document.ORID == null)
                _document.ORID = new ORID();

            _document.ORID.ClusterId = _connection.Database.GetClusters().First(x => x.Name == clusterName).Id;

            return this;
        }

        public OSqlCreateVertexDirect Cluster<T>()
        {
            return Cluster(typeof(T).Name);
        }

        #endregion

        #region Set

        public OSqlCreateVertexDirect Set<T>(string fieldName, T fieldValue)
        {
            _document.SetField(fieldName, fieldValue);

            return this;
        }

        public OSqlCreateVertexDirect Set<T>(T obj)
        {
            var document = obj is ODocument ? obj as ODocument : ODocument.ToDocument(obj);

            // TODO: go also through embedded fields
            foreach (KeyValuePair<string, object> field in document)
            {
                // set only fields which doesn't start with @ character
                if ((field.Key.Length > 0) && (field.Key[0] != '@'))
                {
                    Set(field.Key, field.Value);
                }
            }

            return this;
        }

        #endregion

        #region Run

        public OVertex Run()
        {
            //            var operation = CreateSQLOperation();

            var operation = new CreateRecord(_document, _connection.Database);

            return _connection.ExecuteOperation(operation).To<OVertex>();
        }

       

        public T Run<T>() where T : class, new()
        {
            return Run().To<T>();
        }

        #endregion

      
    }
}
