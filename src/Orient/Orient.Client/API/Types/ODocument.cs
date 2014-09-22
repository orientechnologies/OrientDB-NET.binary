using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Orient.Client.Mapping;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client
{
    public class ODocument : Dictionary<string, object>
    {
        #region Properties which holds orient specific fields

        public ORID ORID 
        { 
            get 
            {
                return this.HasField("@ORID") ? this.GetField<ORID>("@ORID") : null; 
            } 
            set { this.SetField("@ORID", value); }
        }

        public int OVersion 
        {
            get
            {
                return this.GetField<int>("@OVersion");
            }
            set { this.SetField("@OVersion", value); }
        }

        public ORecordType OType
        {
            get
            {
                return this.GetField<ORecordType>("@OType");
            }
            set { this.SetField("@OType", value); }
        }

        public short OClassId
        {
            get
            {
                return this.GetField<short>("@OClassId");
            }
            set { this.SetField("@OClassId", value); }
        }

        public string OClassName
        {
            get
            {
                return this.GetField<string>("@OClassName");
            }
            set { this.SetField("@OClassName", value); }
        }

        #endregion

        public T GetField<T>(string fieldPath)
        {
            Type type = typeof(T);
            T value;

            if (type.IsPrimitive || type.IsArray || (type.Name == "String"))
            {
                value = default(T);
            }
            else
            {
                value = (T)Activator.CreateInstance(type);
            }

            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                ODocument embeddedDocument = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        // if value is collection type, get element type and enumerate over its elements
                        if (value is IList)
                        {
                            Type elementType = ((IEnumerable)value).GetType().GetGenericArguments()[0];
                            IEnumerator enumerator = ((IEnumerable)embeddedDocument[field]).GetEnumerator();

                            while (enumerator.MoveNext())
                            {
                                // if current element is ODocument type which is dictionary<string, object>
                                // map its dictionary data to element instance
                                if (enumerator.Current is ODocument)
                                {
                                    var instance = Activator.CreateInstance(elementType);
                                    ((ODocument)enumerator.Current).Map(ref instance);

                                    ((IList)value).Add(instance);
                                }
                                else
                                {
                                    ((IList)value).Add(enumerator.Current);
                                }
                            }
                        }
                        else if (type.Name == "HashSet`1")
                        {
                            Type elementType = ((IEnumerable)value).GetType().GetGenericArguments()[0];
                            IEnumerator enumerator = ((IEnumerable)this[fieldPath]).GetEnumerator();

                            var addMethod = type.GetMethod("Add");

                            while (enumerator.MoveNext())
                            {
                                // if current element is ODocument type which is Dictionary<string, object>
                                // map its dictionary data to element instance
                                if (enumerator.Current is ODocument)
                                {
                                    var instance = Activator.CreateInstance(elementType);
                                    ((ODocument)enumerator.Current).Map(ref instance);

                                    addMethod.Invoke(value, new object[] { instance });
                                }
                                else
                                {
                                    addMethod.Invoke(value, new object[] { enumerator.Current });
                                }
                            }
                        }
                        else
                        {
                            value = (T)embeddedDocument[field];
                        }
                        break;
                    }

                    if (embeddedDocument.ContainsKey(field))
                    {
                        embeddedDocument = (ODocument)embeddedDocument[field];
                        iteration++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if (this.ContainsKey(fieldPath))
                {
                    // if value is list or set type, get element type and enumerate over its elements
                    if (value is IList)
                    {
                        Type elementType = ((IEnumerable)value).GetType().GetGenericArguments()[0];
                        IEnumerator enumerator = EnumerableFromField<T>(this[fieldPath]).GetEnumerator();
                        
                        while (enumerator.MoveNext())
                        {
                            // if current element is ODocument type which is Dictionary<string, object>
                            // map its dictionary data to element instance
                            if (enumerator.Current is ODocument)
                            {
                                var instance = Activator.CreateInstance(elementType);
                                ((ODocument)enumerator.Current).Map(ref instance);

                                ((IList)value).Add(instance);
                            }
                            else
                            {
                                ((IList)value).Add(enumerator.Current);
                            }
                        }
                    }
                    else if (type.Name == "HashSet`1")
                    {
                        Type elementType = ((IEnumerable)value).GetType().GetGenericArguments()[0];
                        IEnumerator enumerator = ((IEnumerable)this[fieldPath]).GetEnumerator();

                        var addMethod = type.GetMethod("Add");

                        while (enumerator.MoveNext())
                        {
                            // if current element is ODocument type which is Dictionary<string, object>
                            // map its dictionary data to element instance
                            if (enumerator.Current is ODocument)
                            {
                                var instance = Activator.CreateInstance(elementType);
                                ((ODocument)enumerator.Current).Map(ref instance);

                                addMethod.Invoke(value, new object[] { instance });
                            }
                            else
                            {
                                addMethod.Invoke(value, new object[] { enumerator.Current });
                            }
                        }
                    }
                    else
                    {
                        value = (T)this[fieldPath];
                    }
                }
            }

            return value;
        }

        private static IEnumerable EnumerableFromField<T>(object oField)
        {
            if (oField is IEnumerable)
                return ((IEnumerable) oField);
            if (oField == null)
                return (new object[0]);

            return new[] {oField};
        }

        public ODocument SetField<T>(string fieldPath, T value)
        {
            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                ODocument embeddedDocument = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        if (embeddedDocument.ContainsKey(field))
                        {
                            embeddedDocument[field] = value;
                        }
                        else
                        {
                            embeddedDocument.Add(field, value);
                        }
                        break;
                    }

                    if (embeddedDocument.ContainsKey(field))
                    {
                        embeddedDocument = (ODocument)embeddedDocument[field];
                    }
                    else
                    {
                        // if document which contains the field doesn't exist create it first
                        ODocument tempDocument = new ODocument();
                        embeddedDocument.Add(field, tempDocument);
                        embeddedDocument = tempDocument;
                    }

                    iteration++;
                }
            }
            else
            {
                if (this.ContainsKey(fieldPath))
                {
                    this[fieldPath] = value;
                }
                else
                {
                    this.Add(fieldPath, value);
                }
            }

            return this;
        }

        public bool HasField(string fieldPath)
        {
            bool contains = false;

            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                ODocument embeddedDocument = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        contains = embeddedDocument.ContainsKey(field);
                        break;
                    }

                    if (embeddedDocument.ContainsKey(field))
                    {
                        embeddedDocument = (ODocument)embeddedDocument[field];
                        iteration++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                contains = this.ContainsKey(fieldPath);
            }

            return contains;
        }

        public T To<T>() where T : class, new()
        {
            T genericObject = new T();

            genericObject = (T)ToObject<T>(genericObject, "");

            return genericObject;
        }

        public T ToUnique<T>(ICreationContext store) where T : class
        {
            
            if (store.AlreadyCreated(ORID))
                return (T) store.GetExistingObject(ORID);

            T genericObject = (T) store.CreateObject(OClassName);
            var result = ToObject(genericObject, "");
            store.AddObject(ORID, result);
            return result;
        }

        public string Serialize()
        {
            return RecordSerializer.Serialize(this);
        }

        public static ODocument Deserialize(string recordString)
        {
            return RecordSerializer.Deserialize(recordString);
        }

        public static ODocument ToDocument<T>(T genericObject)
        {

            return TypeMapperBase.GetInstanceFor(genericObject.GetType()).ToDocument(genericObject);
        }

        private T ToObject<T>(T genericObject, string path) where T : class
        {
                    
            TypeMapper<T>.Instance.ToObject(this, genericObject);
            return genericObject;

        }
        
        private void Map(ref object obj)
        {
            if (obj is Dictionary<string, object>)
            {
                obj = this;
            }
            else
            {
                Type objType = obj.GetType();

                foreach (KeyValuePair<string, object> item in this)
                {
                    PropertyInfo property = objType.GetProperty(item.Key);

                    if (property != null)
                    {
                        property.SetValue(obj, item.Value, null);
                    }
                }
            }
        }
    }

    public interface ICreationContext
    {
        object GetExistingObject(ORID key);
        void AddObject(ORID key, Object value);
        bool AlreadyCreated(ORID key);

        object CreateObject(string oClassName);
    }
}
