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
            PropertyInfo propertyInfo = mappingType.GetTypeInfo().GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
            return (TypeMapperBase)propertyInfo.GetValue(null, null);
        }


    }

    public class TypeMapper<T> : TypeMapperBase
    {


        private static readonly TypeMapper<T> _instance = new TypeMapper<T>();
        public static TypeMapper<T> Instance { get { return _instance; } }

        readonly List<IFieldMapping> _fields = new List<IFieldMapping>();




        private TypeMapper()
        {
            Type genericObjectType = typeof(T);

            if (typeof(ODocument).GetTypeInfo().IsAssignableFrom(genericObjectType))
            {
                _fields.Add(new AllFieldMapping<T>());
                return;
            }


            foreach (PropertyInfo propertyInfo in genericObjectType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
                    continue; // read only or write only properties can be ignored

                string propertyName = propertyInfo.Name;

                // serialize orient specific fields into dedicated properties
                switch (propertyName)
                {
                    case "ORID":
                        _fields.Add(new ORIDFieldMapping<T>(propertyInfo));
                        continue;
                    case "OVersion":
                        _fields.Add(new OVersionFieldMapping<T>(propertyInfo));
                        continue;
                    case "OType":
                        _fields.Add(new OTypeFieldMapping<T>(propertyInfo));
                        continue;
                    case "OClassId":
                        _fields.Add(new OClassIdFieldMapping<T>(propertyInfo));
                        continue;
                    case "OClassName":
                        _fields.Add(new OClassNameFieldMapping<T>(propertyInfo));
                        continue;
                }


                var oProperty = propertyInfo.GetOPropertyAttribute();

                if (oProperty != null)
                {
                    if (!oProperty.Deserializable)
                        continue;
                    propertyName = oProperty.Alias;
                }

                string fieldPath = propertyName;

                if (propertyInfo.PropertyType.IsArray)
                {
                    if (propertyInfo.PropertyType == typeof(byte[]))
                    {
                        _fields.Add(new BasicNamedFieldMapping<T>(propertyInfo, fieldPath));
                    }
                    else
                    {
                        _fields.Add(new ArrayNamedFieldMapping<T>(propertyInfo, fieldPath));
                    }
                }
                else if (propertyInfo.PropertyType.GetTypeInfo().IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>))
                {
                    if (propertyInfo.PropertyType.Name.StartsWith("List")
                        || propertyInfo.PropertyType.Name.StartsWith("IList"))
                        _fields.Add(new ListNamedFieldMapping<T>(propertyInfo, fieldPath));
                    else if (propertyInfo.PropertyType.Name.StartsWith("HashSet"))
                        _fields.Add(new HashSetNamedFieldMapping<T>(propertyInfo, fieldPath));
                    else if (propertyInfo.PropertyType.Name.StartsWith("Dictionary"))
                        _fields.Add(new DictionaryFieldMapping<T>(propertyInfo, fieldPath));
                    else if (propertyInfo.PropertyType.Name.StartsWith("SortedList"))
                        _fields.Add(new DictionaryFieldMapping<T>(propertyInfo, fieldPath));
                    else
                        throw new NotImplementedException("No mapping implemented for type " + propertyInfo.PropertyType.Name);
                }
                // property is class except the string or ORID type since string and ORID values are parsed differently
                else if (propertyInfo.PropertyType.GetTypeInfo().IsClass &&
                         (propertyInfo.PropertyType.Name != "String") &&
                         (propertyInfo.PropertyType.Name != "ORID"))
                {
                    AddClassProperty(propertyInfo, fieldPath);
                }
                else if (propertyInfo.PropertyType == typeof(Guid))
                {
                    _fields.Add(new GuidFieldMapping<T>(propertyInfo, fieldPath));
                }
                else if (propertyInfo.PropertyType == typeof(DateTime))
                {
                    _fields.Add(new DateTimeFieldMapping<T>(propertyInfo, fieldPath));
                }
                else if (propertyInfo.PropertyType == typeof(TimeSpan))
                {
                    _fields.Add(new TimeSpanFieldMapping<T>(propertyInfo, fieldPath));
                }
                else if (propertyInfo.PropertyType == typeof(Nullable<TimeSpan>))
                {
                    _fields.Add(new NullableTimeSpanFieldMapping<T>(propertyInfo, fieldPath));
                }
                else if (propertyInfo.PropertyType == typeof(long))
                {
                    _fields.Add(new LongFieldMapping<T>(propertyInfo, fieldPath));
                }
                else if (propertyInfo.PropertyType == typeof(Decimal))
                {
                    _fields.Add(new DecimalFieldMapping<T>(propertyInfo, fieldPath));
                }
                else if (propertyInfo.PropertyType == typeof(short))
                {
                    _fields.Add(new ShortFieldMapping<T>(propertyInfo, fieldPath));
                }
                else if (propertyInfo.PropertyType == typeof(long))
                {
                    _fields.Add(new LongFieldMapping<T>(propertyInfo, fieldPath));
                }
                else if (propertyInfo.PropertyType.GetTypeInfo().BaseType == typeof(Enum))
                {
                    _fields.Add(new EnumFieldMapping<T>(propertyInfo, fieldPath));
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
            _fields.Add(new BasicNamedFieldMapping<T>(propertyInfo, fieldPath));
        }

        private void AddClassProperty(PropertyInfo propertyInfo, string fieldPath)
        {
            var mappingType = typeof(ClassFieldMapping<,>).MakeGenericType(propertyInfo.PropertyType, typeof(T));
            _fields.Add((IFieldMapping)Activator.CreateInstance(mappingType, propertyInfo, fieldPath));
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

            if(genericObject != null)
            {
                foreach (var fm in _fields)
                    fm.MapToDocument(genericObject, document);

                if (string.IsNullOrEmpty(document.OClassName))
                    document.OClassName = genericObject.GetType().Name;
            }

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
