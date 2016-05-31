using System.Collections.Generic;
using System.Linq;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;
using Orient.Client.Protocol.Operations.Command;

namespace Orient.Client.API.Query
{
    public class OSqlSchema
    {
        private Connection _connection;
        private string query = "select expand(classes) from metadata:schema";
        private IEnumerable<ODocument> _schema;

        internal OSqlSchema(Connection connection)
        {
            _connection = connection;
            _schema = Run();
        }

        public IEnumerable<string> Classes()
        {
            return _schema.Select(d => d.GetField<string>("name"));
        }

        public IEnumerable<ODocument> Properties(string @class)
        {
            var pDocument = _schema.FirstOrDefault(d => d.GetField<string>("name") == @class);
            return pDocument != null ? pDocument.GetField<HashSet<ODocument>>("properties") : null;
        }

        public bool IsClassExist(string @class)
        {
            var pDocument = _schema.FirstOrDefault(d => d.GetField<string>("name") == @class);
            return (pDocument != null);
        }

        public bool IsClassExist<T>()
        {
            var @class = typeof(T).Name;
            return IsClassExist(@class);
        }

        public IEnumerable<ODocument> Properties<T>()
        {
            var @class = typeof(T).Name;
            return Properties(@class);
        }

        public short GetDefaultClusterForClass(string @class)
        {
            var pDocument = _schema.FirstOrDefault(d => d.GetField<string>("name") == @class);
            return pDocument != null ? (short)pDocument.GetField<int>("defaultClusterId") : (short)-1;
        }

        public short GetDefaultClusterForClass<T>()
        {
            var @class = typeof(T).Name;
            return GetDefaultClusterForClass(@class);
        }

        public IEnumerable<int> GetClustersForClass(string @class)
        {
            var pDocument = _schema.FirstOrDefault(d => d.GetField<string>("name") == @class);
            return pDocument != null ? pDocument.GetField<List<int>>("clusterIds") : null;
        }

        public IEnumerable<int> GetClustersForClass<T>()
        {
            var @class = typeof(T).Name;
            return GetClustersForClass(@class);
        }

        private IEnumerable<ODocument> Run()
        {
            CommandPayloadQuery payload = new CommandPayloadQuery();
            payload.Text = query;
            payload.NonTextLimit = -1;
            payload.FetchPlan = "*:0";

            Command operation = new Command(_connection.Database);
            operation.OperationMode = OperationMode.Asynchronous;
            operation.CommandPayload = payload;

            ODocument document = _connection.ExecuteOperation(operation);

            return document.GetField<List<ODocument>>("Content");

        }
    }
}
