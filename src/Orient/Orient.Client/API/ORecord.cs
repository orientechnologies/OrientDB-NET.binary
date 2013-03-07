using Orient.Client.Protocol;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client
{
    public class ORecord
    {
        internal DataObject DataObject { get; set; }

        public ORID ORID { get; set; }
        public ORecordType Type { get; set; }
        public int Version { get; set; }
        public short ClassId { get; set; }
        public string ClassName { get; set; }

        public ORecord()
        {
            ORID = new ORID();
            DataObject = new DataObject();
        }

        public ORecord(ORID orid, int version, ORecordType type, short classId)
        {
            ORID = orid;
            Version = version;
            Type = type;
            ClassId = classId;
            DataObject = new DataObject();
        }

        /*public ORecord()
        {
            Fields = new Dictionary<string, object>();
        }

        public ORecord(ORecordType type, int version, byte[] content)
        {
            Type = type;
            Version = version;
            Content = content;
            Fields = new Dictionary<string, object>();

            Deserialize();
        }

        internal ORecord(DtoRecord record)
        {
            ORID = record.ORID;
            Type = record.Type;
            Version = record.Version;
            Content = record.Content;
            Fields = new Dictionary<string, object>();
        }

        public static byte[] Serialize<T>(T o)
        {
            return RecordSerializer.ToArray(o, o.GetType());
        }

        public T ToObject<T>() where T : class, new()
        {
            T genericObject = new T();

            genericObject = (T)ToObject(genericObject, Fields);

            return genericObject;
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
        }

        private object ToObject(object genericObject, Dictionary<string, object> fields)
        {
            Type genericObjectType = genericObject.GetType();

            foreach (KeyValuePair<string, object> item in fields)
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

                                            ((IList)collectionInstance).Add(ToObject(instance, (Dictionary<string, object>)collection[i]));
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

                            property.SetValue(genericObject, ToObject(instance, (Dictionary<string, object>)item.Value), null);
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
        }*/
    }
}
