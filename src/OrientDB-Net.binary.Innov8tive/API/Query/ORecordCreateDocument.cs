using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.API.Query.Interfaces;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

namespace Orient.Client.API.Query
{
    public class ORecordCreateDocument : IOCreateDocument
    {
        private Connection _connection;
        private ODocument _document;

        public ORecordCreateDocument()
        {
        }

        internal ORecordCreateDocument(Connection connection)
        {
            _connection = connection;
            _document = new ODocument();
        }

        public IOCreateDocument Cluster(string clusterName)
        {
            throw new NotImplementedException();
        }

        public IOCreateDocument Cluster<T>()
        {
            throw new NotImplementedException();
        }

        public IOCreateDocument Document(string className)
        {
            _document.OClassName = className;
            return this;
        }

        public IOCreateDocument Document<T>()
        {
            return Document(typeof(T).Name);
        }

        public IOCreateDocument Document<T>(T obj)
        {
            if (obj is ODocument)
            {
                _document = obj as ODocument;
            }
            else
            {
                _document = ODocument.ToDocument(obj);
            }
            return this;
        }

        public ODocument Run()
        {
            var operation = new RecordCreate(_document, _connection.Database);
            operation.OperationMode = OperationMode.Synchronous;

            return _connection.ExecuteOperation(operation);
        }

        public T Run<T>() where T : class, new()
        {
            return Run().To<T>();
        }

        public IOCreateDocument Set<T>(string fieldName, T fieldValue)
        {
            _document.SetField<T>(fieldName, fieldValue);

            return this;
        }

        public IOCreateDocument Set<T>(T obj)
        {
            if (obj is ODocument)
            {
                _document = obj as ODocument;
            }
            else
            {
                _document = ODocument.ToDocument(obj);
            }

            return this;
        }
    }
}
