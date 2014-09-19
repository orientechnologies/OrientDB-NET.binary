using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Orient.Client.Mapping
{
    public class TypeMapper<T>
    {


        private static readonly TypeMapper<T> _instance = new TypeMapper<T>();
        public static TypeMapper<T> Instance { get { return _instance; } }

        readonly List<FieldMapping> _fields = new List<FieldMapping>();

        private TypeMapper()
        {
            Type genericObjectType = typeof (T);

            if (genericObjectType.Name.Equals("ODocument") ||
                genericObjectType.Name.Equals("OVertex") ||
                genericObjectType.Name.Equals("OEdge"))
            {
                _fields.Add(new AllFieldMapping());
                return;
            }

            string path = "";
            foreach (PropertyInfo propertyInfo in genericObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
                    continue; // read only or write only properties can be ignored

                string propertyName = propertyInfo.Name;

                // serialize orient specific fields into dedicated properties
                switch (propertyName)
                {
                    case "ORID":
                        _fields.Add(new ORIDFieldMapping(propertyInfo));
                        continue;
                    case "OVersion":
                        _fields.Add(new OVersionFieldMapping(propertyInfo));
                        continue;
                    case "OClassId":
                        _fields.Add(new OClassIdFieldMapping(propertyInfo));
                        continue;
                    case "OClassName":
                        _fields.Add(new OClassNameFieldMapping(propertyInfo));
                        continue;
                }

                object[] oProperties = propertyInfo.GetCustomAttributes(typeof(OProperty), true);

                if (oProperties.Any())
                {
                    OProperty oProperty = oProperties.First() as OProperty;
                    if (oProperty != null)
                    {
                        if (!oProperty.Deserializable)
                            continue;
                        propertyName = oProperty.Alias;
                    }
                }

                string fieldPath = path + (path != "" ? "." : "") + propertyName;

                if ((propertyInfo.PropertyType.IsArray || propertyInfo.PropertyType.IsGenericType))
                {
                    AddCollectionProperrty(propertyInfo, fieldPath);
                }
                    // property is class except the string or ORID type since string and ORID values are parsed differently
                else if (propertyInfo.PropertyType.IsClass &&
                         (propertyInfo.PropertyType.Name != "String") &&
                         (propertyInfo.PropertyType.Name != "ORID"))
                {
                    AddClassProperty(propertyInfo, fieldPath);
                }
                    // property is basic type
                else
                {
                    AddBasicProperty(propertyInfo, fieldPath);
                }
            }

        }

        private void AddBasicProperty(PropertyInfo propertyInfo, string fieldPath)
        {
            _fields.Add(new BasicNamedFieldMapping(propertyInfo, fieldPath));
        }

        private void AddClassProperty(PropertyInfo propertyInfo, string fieldPath)
        {
            var mappingType = typeof (ClassFieldMapping<>).MakeGenericType(propertyInfo.PropertyType);
            _fields.Add((FieldMapping) Activator.CreateInstance(mappingType, propertyInfo, fieldPath));
        }

        private void AddCollectionProperrty(PropertyInfo propertyInfo, string fieldPath)
        {
            _fields.Add(new CollectionNamedFieldMapping(propertyInfo, fieldPath));
        }

        public void ToObject(ODocument document, T typedObject, string basePath = null)
        {
            foreach (var fm in _fields)
                fm.MapToObject(document, typedObject, basePath);
        }
    }
}
