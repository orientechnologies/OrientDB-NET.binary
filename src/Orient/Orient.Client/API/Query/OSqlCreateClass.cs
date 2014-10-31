using System;
using System.Collections.Generic;
using System.Reflection;
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
        private SqlQuery _sqlQuery = new SqlQuery();
        private Connection _connection;
        private string _className;
        private Type _type;
        private bool _autoProperties;

        public OSqlCreateClass()
        {
        }

        internal OSqlCreateClass(Connection connection)
        {
            _connection = connection;
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
            CommandPayloadCommand payload = new CommandPayloadCommand();
            payload.Text = ToString();

            Command operation = new Command();
            operation.OperationMode = OperationMode.Synchronous;
            operation.CommandPayload = payload;

            OCommandResult result = new OCommandResult(_connection.ExecuteOperation(operation));

            var clusterId = short.Parse(result.ToDocument().GetField<string>("Content"));

            _connection.Database.AddCluster(new OCluster { Name = _className, Id = clusterId });

            if (_autoProperties)
            {
                CreateAutoProperties();
            }

            return clusterId;
        }

        private void CreateAutoProperties()
        {
            foreach (var pi in _type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
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
            string propType = ConvertPropertyType(pi.PropertyType);
            _connection.Database.Command(string.Format("create property {2}.{0} {1}", pi.Name, propType, _type.Name));
        }

        private string ConvertPropertyType(Type propertyType)
        {
            return TypeConverter.TypeToDbName(propertyType);
        }

        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.CreateClass);
        }

        class TypeConverter
        {
            static TypeConverter()
            {
                AddType<int>("Integer");
                AddType<long>("Long");
                AddType<short>("Short");
                AddType<string>("string");
                AddType<bool>("Boolean");
                AddType<float>("Float");
                AddType<double>("Double");
                AddType<DateTime>("Datetime");
                AddType<byte[]>("Binary");
                AddType<byte>("Byte");
                AddType<List<ORID>>("LinkList");
                AddType<ORID>("Link");
            }

            private static void AddType<T>(string name)
            {
                _types.Add(typeof(T), name);
            }

            static Dictionary<Type, string> _types = new Dictionary<Type, string>();

            public static string TypeToDbName(Type t)
            {
                string result;
                if (_types.TryGetValue(t, out result))
                    return result;

                throw new ArgumentException("propertyType " + t.Name + " is not yet supported.");
            }
        }
    }
}
