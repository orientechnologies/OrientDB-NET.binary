using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Orient.Client.Mapping
{
    public abstract class TypeMapperBase
    {
        public abstract void ToObject(ODocument document, object typedObject);
        public abstract ODocument ToDocument(object typedObject);

        public static TypeMapperBase GetInstanceFor(Type t)
        {
            var mappingType = typeof(TypeMapper<>).MakeGenericType(t);
            PropertyInfo propertyInfo = mappingType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
            return (TypeMapperBase) propertyInfo.GetValue(null, null);
        }


    }

    public class TypeMapper<T> : TypeMapperBase
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

                string fieldPath = propertyName;

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

        public void ToObject(ODocument document, T typedObject)
        {
            foreach (var fm in _fields)
                fm.MapToObject(document, typedObject);
        }

        public override void ToObject(ODocument document, object typedObject)
        {
            ToObject(document, (T)typedObject);
        }

        public override ODocument ToDocument(object genericObject)
        {
            ODocument document = new ODocument();

            foreach (var fm in _fields)
                fm.MapToDocument(genericObject, document);

            if (string.IsNullOrEmpty(document.OClassName))
                document.OClassName = genericObject.GetType().Name;

            return document;

            //Type genericObjectType = genericObject.GetType();

            //// TODO: recursive mapping of nested/embedded objects
            //foreach (PropertyInfo propertyInfo in genericObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            //{
            //    if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
            //        continue; // read only or write only properties can be ignored

            //    string propertyName = propertyInfo.Name;

            //    // serialize following properties into dedicated fields in ODocument
            //    if (propertyName.Equals("ORID"))
            //    {
            //        document.ORID = (ORID)propertyInfo.GetValue(genericObject, null);
            //        continue;
            //    }
            //    else if (propertyName.Equals("OVersion"))
            //    {
            //        document.OVersion = (int)propertyInfo.GetValue(genericObject, null);
            //        continue;
            //    }
            //    else if (propertyName.Equals("OClassId"))
            //    {
            //        document.OClassId = (short)propertyInfo.GetValue(genericObject, null);
            //        continue;
            //    }
            //    else if (propertyName.Equals("OClassName"))
            //    {
            //        document.OClassName = (string)propertyInfo.GetValue(genericObject, null);
            //        continue;
            //    }

            //    bool isSerializable = true;
            //    object[] oProperties = propertyInfo.GetCustomAttributes(typeof(OProperty), true);

            //    if (oProperties.Any())
            //    {
            //        OProperty oProperty = oProperties.First() as OProperty;
            //        if (oProperty != null)
            //        {
            //            propertyName = oProperty.Alias;
            //            isSerializable = oProperty.Serializable;
            //        }
            //    }

            //    if (isSerializable)
            //    {
            //        document.SetField(propertyName, propertyInfo.GetValue(genericObject, null));
            //    }
            //}

      

            //return document;
        }
    }
}
