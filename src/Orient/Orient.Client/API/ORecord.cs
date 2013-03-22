using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client
{
    public class ORecord
    {
        internal ODataObject DataObject { get; set; }

        public ORID ORID { get; set; }
        public ORecordType Type { get; set; }
        public int Version { get; set; }
        public short ClassId { get; set; }
        public string ClassName { get; set; }

        public ORecord()
        {
            ORID = new ORID();
            DataObject = new ODataObject();
        }

        public ORecord(string recordString)
        {
            Deserialize(recordString);
        }

        public ORecord(ORID orid, int version, ORecordType type, short classId)
        {
            ORID = orid;
            Version = version;
            Type = type;
            ClassId = classId;
            DataObject = new ODataObject();
        }

        public bool HasField(string fieldPath)
        {
            return DataObject.Has(fieldPath);
        }

        public T GetField<T>(string fieldPath)
        {
            return DataObject.Get<T>(fieldPath);
        }

        public void SetField<T>(string fieldPath, T value)
        {
            DataObject.Set<T>(fieldPath, value);
        }

        public override string ToString()
        {
            string record = string.Format("{0}, {1}, v{2}, {3} ({4})", ORID.ToString(), Type, Version, ClassName, ClassId);

            return record;
        }

        public T To<T>() where T : class, new()
        {
            T genericObject = new T();

            genericObject = (T)ToObject<T>(genericObject, "");

            return genericObject;
        }

        public void Deserialize(string recordString)
        {
            ORecord record =  RecordSerializer.ToRecord(recordString);
            
            DataObject = record.DataObject;

            ORID = record.ORID;
            Type = record.Type;
            Version = record.Version;
            ClassId = record.ClassId;
            ClassName = record.ClassName;
        }

        private T ToObject<T>(T genericObject, string path) where T : class, new()
        {
            Type genericObjectType = genericObject.GetType();

            foreach (PropertyInfo propertyInfo in genericObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string propertyName = propertyInfo.Name;
                OProperty oProperty = propertyInfo.GetCustomAttribute<OProperty>();

                if (oProperty != null)
                {
                    propertyName = oProperty.MappedTo;
                }

                string fieldPath = path + (path != "" ? "." : "") + propertyName;

                if ((propertyInfo.PropertyType.IsArray || propertyInfo.PropertyType.IsGenericType))
                {
                    if (DataObject.Has(fieldPath))
                    {
                        object propertyValue = DataObject.Get<object>(fieldPath);

                        IList collection = (IList)propertyValue;

                        if (collection.Count > 0)
                        {
                            // create instance of property type
                            object collectionInstance = Activator.CreateInstance(propertyInfo.PropertyType, collection.Count);

                            for (int i = 0; i < collection.Count; i++)
                            {
                                // collection is simple array
                                if (propertyInfo.PropertyType.IsArray)
                                {
                                    ((object[])collectionInstance)[i] = collection[i];
                                }
                                // collection is generic
                                else if (propertyInfo.PropertyType.IsGenericType && (propertyValue is IEnumerable))
                                {
                                    Type elementType = collection[i].GetType();

                                    // generic collection consists of basic types or ORIDs
                                    if (elementType.IsPrimitive ||
                                        (elementType == typeof(string)) ||
                                        (elementType == typeof(DateTime)) ||
                                        (elementType == typeof(decimal)) ||
                                        (elementType == typeof(ORID)))
                                    {
                                        ((IList)collectionInstance).Add(collection[i]);
                                    }
                                    // generic collection consists of generic type which should be parsed
                                    else
                                    {
                                        // create instance object based on first element of generic collection
                                        object instance = Activator.CreateInstance(propertyInfo.PropertyType.GetGenericArguments().First(), null);

                                        ((IList)collectionInstance).Add(ToObject(instance, fieldPath));
                                    }
                                }
                                else
                                {
                                    object v = Activator.CreateInstance(collection[i].GetType(), collection[i]);

                                    ((IList)collectionInstance).Add(v);
                                }
                            }

                            propertyInfo.SetValue(genericObject, collectionInstance, null);
                        }
                    }
                }
                // property is class except the string or ORID type since string and ORID values are parsed differently
                else if (propertyInfo.PropertyType.IsClass &&
                    (propertyInfo.PropertyType.Name != "String") &&
                    (propertyInfo.PropertyType.Name != "ORID"))
                {
                    // create object instance of embedded class
                    object instance = Activator.CreateInstance(propertyInfo.PropertyType);

                    propertyInfo.SetValue(genericObject, ToObject(instance, fieldPath), null);
                }
                // property is basic type
                else
                {
                    if (DataObject.Has(fieldPath))
                    {
                        object propertyValue = DataObject.Get<object>(fieldPath);

                        propertyInfo.SetValue(genericObject, propertyValue, null);
                    }
                }
            }

            return genericObject;
        }

        /*private object ToObject(object genericObject, DataObject dataObject)
        {
            Type genericObjectType = genericObject.GetType();

            foreach (KeyValuePair<string, object> item in dataObject)
            {
                PropertyInfo property = genericObjectType.GetProperty(item.Key, BindingFlags.Public | BindingFlags.Instance);

                if (property != null)
                {
                    // field item need to have a value otherwise property is set to null
                    if (item.Value != null)
                    {
                        // property is array or generic collection 
                        if ((property.PropertyType.IsArray || property.PropertyType.IsGenericType))
                        {
                            IList collection = (IList)item.Value;

                            if (collection.Count > 0)
                            {
                                // create instance of property type
                                object collectionInstance = Activator.CreateInstance(property.PropertyType, collection.Count);

                                for (int i = 0; i < collection.Count; i++)
                                {
                                    // collection is simple array
                                    if (property.PropertyType.IsArray)
                                    {
                                        ((object[])collectionInstance)[i] = collection[i];
                                    }
                                    // collection is generic
                                    else if (property.PropertyType.IsGenericType && (item.Value is IEnumerable))
                                    {
                                        Type elementType = collection[i].GetType();

                                        // generic collection consists of basic types or ORIDs
                                        if (elementType.IsPrimitive ||
                                            (elementType == typeof(string)) ||
                                            (elementType == typeof(DateTime)) ||
                                            (elementType == typeof(decimal)) ||
                                            (elementType == typeof(ORID)))
                                        {
                                            ((IList)collectionInstance).Add(collection[i]);
                                        }
                                        // generic collection consists of generic type which should be parsed
                                        else
                                        {
                                            // create instance object based on first element of generic collection
                                            object instance = Activator.CreateInstance(property.PropertyType.GetGenericArguments().First(), null);

                                            ((IList)collectionInstance).Add(ToObject(instance, (DataObject)collection[i]));
                                        }
                                    }
                                    else
                                    {
                                        object v = Activator.CreateInstance(collection[i].GetType(), collection[i]);

                                        ((IList)collectionInstance).Add(v);
                                    }
                                }

                                property.SetValue(genericObject, collectionInstance, null);
                            }
                        }
                        // property is class except the string or ORID type since string and ORID values are parsed differently
                        else if (property.PropertyType.IsClass &&
                            (property.PropertyType.Name != "String") &&
                            (property.PropertyType.Name != "ORID"))
                        {
                            // create object instance of embedded class
                            object instance = Activator.CreateInstance(property.PropertyType);

                            property.SetValue(genericObject, ToObject(instance, (DataObject)item.Value), null);
                        }
                        // property is basic type
                        else
                        {
                            property.SetValue(genericObject, item.Value, null);
                        }
                    }
                    else
                    {
                        property.SetValue(genericObject, null, null);
                    }
                }
            }

            return genericObject;
        }

        /*public static byte[] Serialize<T>(T o)
        {
            return RecordSerializer.ToArray(o, o.GetType());
        }

        // for testing parser logic
        private void Deserialize()
        {
            DtoRecord record = new DtoRecord();
            record.Type = Type;
            record.Content = Content;

            if (Type == ORecordType.Document)
            {
                ORecord deserializedRecord = RecordSerializer.DeserializeRecord(record);

                Class = deserializedRecord.Class;
                Fields = deserializedRecord.Fields;
            }
        }*/
    }
}
