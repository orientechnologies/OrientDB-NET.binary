using System;
using System.Collections.Generic;
using System.Reflection;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

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
            _type = typeof (T);
            _className = typeof (T).Name;
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

            _type = typeof (T);

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
            CommandPayload payload = new CommandPayload();
            payload.Type = CommandPayloadType.Sql;
            payload.Text = ToString();
            payload.NonTextLimit = -1;
            payload.FetchPlan = "";
            payload.SerializedParams = new byte[] { 0 };

            Command operation = new Command();
            operation.OperationMode = OperationMode.Synchronous;
            operation.ClassType = CommandClassType.NonIdempotent;
            operation.CommandPayload = payload;

            OCommandResult result = new OCommandResult(_connection.ExecuteOperation(operation));

            var clusterId = short.Parse(result.ToDocument().GetField<string>("Content"));

            _connection.Database.AddCluster(_className, clusterId);

            if (_autoProperties)
            {
                CreateAutoProperties();
            }

            return clusterId;
        }

        private void CreateAutoProperties()
        {
            foreach (var pi in _type.GetProperties(BindingFlags.DeclaredOnly))
            {
                if (pi.CanRead && pi.CanWrite)
                {
                    if (pi.PropertyType.IsPrimitive)
                    {
                        CreateProperty(pi);
                    }
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
            switch (propertyType.Name)
            {
                case "Int64":
                    return "Long";
                case "String":
                    return "String";
                case "Int32":
                    return "Integer";
            }
            throw new ArgumentException("propertyType " + propertyType.Name + " is not yet supported.");
        }

        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.CreateClass);
        }
    }
}
