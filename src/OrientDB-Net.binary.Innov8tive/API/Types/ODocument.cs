using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Orient.Client.API.Types;
using Orient.Client.Mapping;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client
{
    public class ODocument : Dictionary<string, object>, IBaseRecord
    {
        private static ConcurrentDictionary<Type, bool> _implementsMap = new ConcurrentDictionary<Type, bool>();

        #region Properties which holds orient specific fields

        public ORID ORID
        {
            get
            {
                return HasField("@ORID") ? GetField<ORID>("@ORID") : null;
            }
            set { SetField("@ORID", value); }
        }

        public int OVersion
        {
            get
            {
                return GetField<int>("@OVersion");
            }
            set { SetField("@OVersion", value); }
        }

        public ORecordType OType
        {
            get
            {
                return GetField<ORecordType>("@OType");
            }
            set { SetField("@OType", value); }
        }

        public short OClassId
        {
            get
            {
                return GetField<short>("@OClassId");
            }
            set { SetField("@OClassId", value); }
        }

        public string OClassName
        {
            get
            {
                return GetField<string>("@OClassName");
            }
            set { SetField("@OClassName", value); }
        }

        #endregion

        public T GetField<T>(string fieldPath)
        {
            var type = typeof(T);

            object fieldValue;

            if (TryGetValue(fieldPath, out fieldValue))
            {
                if (fieldValue == null || fieldValue.GetType() == typeof(T))
                    return (T)fieldValue;

                if (fieldValue is ICollection && (fieldValue as ICollection).Count == 1)
                {
                    var enumerable = (fieldValue as IEnumerable).GetEnumerator();
                    enumerable.MoveNext();
                    if (enumerable.Current != null && enumerable.Current is T)
                        return (T)enumerable.Current;
                }

                // if value is list or set type, get element type and enumerate over its elements
                if (!type.GetTypeInfo().IsPrimitive && ImplementsIList(type) && !type.IsArray)
                {
                    var value = (T)Activator.CreateInstance(type);
                    Type elementType = type.GetTypeInfo().GetGenericArguments()[0];
                    IEnumerator enumerator = EnumerableFromField<T>(fieldValue).GetEnumerator();

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
                            try
                            {
                                ((IList)value).Add(enumerator.Current);
                            }
                            catch
                            {
                                ((IList)value).Add(Convert.ChangeType(enumerator.Current, elementType));
                            }
                        }
                    }

                    return value;
                }

                if (type.Name == "HashSet`1")
                {
                    var value = (T)Activator.CreateInstance(type);

                    Type elementType = ((IEnumerable)value).GetType().GetTypeInfo().GetGenericArguments()[0];
                    IEnumerator enumerator = ((IEnumerable)fieldValue).GetEnumerator();

                    var addMethod = type.GetTypeInfo().GetMethod("Add");

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
                    return value;
                }
                else if (type == typeof(DateTime))
                {
                    DateTime parsedValue;
                    if (DateTime.TryParse((string)fieldValue, out parsedValue))
                    {
                        return (T)(object)parsedValue;
                    }
                }
                else if (type == typeof(TimeSpan) || type == typeof(Nullable<TimeSpan>))
                {
                    if (fieldValue != null && (fieldValue.GetType() == typeof(TimeSpan) || fieldValue.GetType() == typeof(Nullable<TimeSpan>)))
                        return (T)fieldValue;
                    TimeSpan parsedValue;
                    if (TimeSpan.TryParse((string)fieldValue, out parsedValue))
                    {
                        return (T)(object)parsedValue;
                    }
                }
                else if (type == typeof(Guid))
                {
                    Guid parsedValue;
                    if (Guid.TryParse((string)fieldValue, out parsedValue))
                    {
                        return (T)(object)parsedValue;
                    }
                }
                else if (type == typeof(Decimal))
                {
                    if (fieldValue != null)
                        return (T)(object)Convert.ChangeType(fieldValue, typeof(T));
                    else
                        return (T)(object)null;
                }

                return (T)fieldValue;
            }
            if (fieldPath.Contains("."))
            {
                ODocument target = this;
                var fields = fieldPath.Split('.');
                for (int i = 0; i < fields.Length - 1; i++)
                {
                    target = target.GetField<ODocument>(fields[i]);
                }
                return target.GetField<T>(fields.Last());
            }

            var result = type.GetTypeInfo().IsPrimitive || type == typeof(string) || type.IsArray ? default(T) : (T)Activator.CreateInstance(type);
            SetField(fieldPath, result);
            return result;
        }

        private bool ImplementsIList(Type type)
        {
            bool result;
            if (_implementsMap.TryGetValue(type, out result))
                return result;

            result = type.GetTypeInfo().GetInterfaces().Contains(typeof(IList));
            _implementsMap.AddOrUpdate(type, type1 => result, (type1, b) => result);
            return result;
        }

        private static IEnumerable EnumerableFromField<T>(object oField)
        {
            if (oField is IEnumerable)
                return ((IEnumerable)oField);
            if (oField == null)
                return (new object[0]);

            return new[] { oField };
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
                if (ContainsKey(fieldPath))
                {
                    this[fieldPath] = value;
                }
                else
                {
                    Add(fieldPath, value);
                }
            }

            return this;
        }

        public bool HasField(string fieldPath)
        {

            if (ContainsKey(fieldPath))
                return true;

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
                return (T)store.GetExistingObject(ORID);

            T genericObject = (T)store.CreateObject(OClassName);
            var result = ToObject(genericObject, "");
            store.AddObject(ORID, result);
            return result;
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
                    PropertyInfo property = objType.GetTypeInfo().GetProperty(item.Key);

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
