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
        internal ODocument Document { get; set; }

        public ORID ORID { get; set; }
        public ORecordType Type { get; set; }
        public int Version { get; set; }
        public short ClassId { get; set; }
        public string ClassName { get; set; }

        public ORecord()
        {
            ORID = new ORID();
            Document = new ODocument();
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
            Document = new ODocument();
        }

        public bool HasField(string fieldPath)
        {
            return Document.HasField(fieldPath);
        }

        public T GetField<T>(string fieldPath)
        {
            return Document.GetField<T>(fieldPath);
        }

        public void SetField<T>(string fieldPath, T value)
        {
            Document.SetField<T>(fieldPath, value);
        }

        public ODocument ToDocument()
        {
            return Document;
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

        public string Serialize()
        {
            return RecordSerializer.ToString(ClassName, Document);
        }

        public void Deserialize(string recordString)
        {
            ORecord record =  RecordSerializer.ToRecord(recordString);

            Document = record.Document;

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
                    if (Document.HasField(fieldPath))
                    {
                        object propertyValue = Document.GetField<object>(fieldPath);

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
                    if (Document.HasField(fieldPath))
                    {
                        object propertyValue = Document.GetField<object>(fieldPath);

                        propertyInfo.SetValue(genericObject, propertyValue, null);
                    }
                }
            }

            return genericObject;
        }
    }
}
