using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Orient.Client.API.Types;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;
using Orient.Client.Protocol.Operations.Command;

// syntax: 
// CREATE CLASS <class> 
// [EXTENDS <super-class>] 
// [CLUSTER <clusterId>*]

namespace Orient.Client
{
    public class OSqlCreateClass
    {
        private SqlQuery _sqlQuery;
        private Connection _connection;
        private string _className;
        private Type _type;
        private bool _autoProperties;
        public OSqlCreateClass()
        {
            _sqlQuery = new SqlQuery(null);
        }
        internal OSqlCreateClass(Connection connection)
        {
            _connection = connection;
            _sqlQuery = new SqlQuery(connection);
        }

        #region Class

        public OSqlCreateClass Class(string className)
        {
            _className = className;
            _sqlQuery.Class(_className);

            return this;
        }

        public OSqlCreateClass Class<T>()
        {
            _type = typeof(T);
            _className = typeof(T).Name;
            return Class(_className);
        }

        public OSqlCreateClass Class<T>(string className)
        {
            _type = typeof(T);
            _className = className;
            return Class(_className);
        }

        #endregion

        #region Extends

        public OSqlCreateClass Extends(string superClass)
        {
            _sqlQuery.Extends(superClass);

            return this;
        }

        public OSqlCreateClass CreateProperties()
        {
            if (_type == null)
                throw new InvalidOperationException("Can only create properties automatically when a generic type parameter has been specified");

            _autoProperties = true;
            return this;
        }

        public OSqlCreateClass CreateProperties<T>()
        {
            if (_type != null && _type != typeof(T))
                throw new InvalidOperationException("Inconsistent type specified - type for CreateProperties<T> must match type for Class<T>");

            _type = typeof(T);

            _autoProperties = true;
            return this;
        }


        public OSqlCreateClass Extends<T>()
        {
            return Extends(typeof(T).Name);
        }

        #endregion

        public OSqlCreateClass Cluster(short clusterId)
        {
            _sqlQuery.Cluster(clusterId.ToString());

            return this;
        }

        public short Run()
        {
            OCluster cluster;
            var defaultClusterId = _connection.Database.GetClusterIdFor(_className);

            if (defaultClusterId == -1)
            {
                CommandPayloadCommand payload = new CommandPayloadCommand();
                payload.Text = ToString();

                Command operation = new Command(_connection.Database);
                operation.OperationMode = OperationMode.Synchronous;
                operation.CommandPayload = payload;

                OCommandResult result = new OCommandResult(_connection.ExecuteOperation(operation));

                var clusterId = short.Parse(result.ToDocument().GetField<string>("Content"));

                cluster = _connection.Database.AddCluster(new OCluster { Name = _className, Id = clusterId });
            }
            else
            {
                cluster = _connection.Database.GetClusters().FirstOrDefault(c => c.Name == _className);
            }

            if (_autoProperties)
            {
                CreateAutoProperties();
            }

            return cluster.Id;
        }

        private void CreateAutoProperties()
        {
            foreach (var pi in _type.GetTypeInfo().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
            {
                if (pi.CanRead && pi.CanWrite)
                {
                    var oprop = pi.GetOPropertyAttribute();
                    if (oprop != null && !oprop.Deserializable && !oprop.Serializable)
                        continue;

                    CreateProperty(pi);
                }
            }
        }

        private void CreateProperty(PropertyInfo pi)
        {
            var propType = ConvertPropertyType(pi.PropertyType);
            var @class =  _className;

            var propid = _connection.Database
                .Create
                .Property(pi.Name, propType)
                .Class(@class)
                .Run();
        }

        private OType ConvertPropertyType(Type propertyType)
        {
            return TypeConverter.TypeToDbName(propertyType);
        }

        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.CreateClass);
        }
    }
}
