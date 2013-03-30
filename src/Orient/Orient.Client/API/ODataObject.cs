using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Orient.Client
{
    public class ODataObject : Dictionary<string, object>
    {
        public T Get<T>(string fieldPath)
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
                ODataObject innerObject = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        // if value is collection type, get element type and enumerate over its elements
                        if (value is IList)
                        {
                            Type elementType = ((IEnumerable)value).GetType().GetGenericArguments()[0];
                            IEnumerator enumerator = ((IEnumerable)innerObject[field]).GetEnumerator();

                            while (enumerator.MoveNext())
                            {
                                // if current element is DataObject type which is dictionary<string, object>
                                // map its dictionary data to element instance
                                if (enumerator.Current is ODataObject)
                                {
                                    var instance = Activator.CreateInstance(elementType);
                                    ((ODataObject)enumerator.Current).MapData(ref instance);

                                    ((IList)value).Add(instance);
                                }
                                else
                                {
                                    ((IList)value).Add(enumerator.Current);
                                }
                            }
                        }
                        else
                        {
                            value = (T)innerObject[field];
                        }
                        break;
                    }

                    if (innerObject.ContainsKey(field))
                    {
                        innerObject = (ODataObject)innerObject[field];
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
                    // if value is collection type, get element type and enumerate over its elements
                    if (value is IList)
                    {
                        Type elementType = ((IEnumerable)value).GetType().GetGenericArguments()[0];
                        IEnumerator enumerator = ((IEnumerable)this[fieldPath]).GetEnumerator();
                        
                        while (enumerator.MoveNext())
                        {
                            // if current element is DataObject type which is dictionary<string, object>
                            // map its dictionary data to element instance
                            if (enumerator.Current is ODataObject)
                            {
                                var instance = Activator.CreateInstance(elementType);
                                ((ODataObject)enumerator.Current).MapData(ref instance);

                                ((IList)value).Add(instance);
                            }
                            else
                            {
                                ((IList)value).Add(enumerator.Current);
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

        public void Set<T>(string fieldPath, T value)
        {
            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                ODataObject innerObject = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        if (innerObject.ContainsKey(field))
                        {
                            innerObject[field] = value;
                        }
                        else
                        {
                            innerObject.Add(field, value);
                        }
                        break;
                    }

                    if (innerObject.ContainsKey(field))
                    {
                        innerObject = (ODataObject)innerObject[field];
                    }
                    else
                    {
                        ODataObject temoObject = new ODataObject();
                        innerObject.Add(field, temoObject);
                        innerObject = temoObject;
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
        }

        public bool Has(string fieldPath)
        {
            bool contains = false;

            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                ODataObject innerObject = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        contains = innerObject.ContainsKey(field);
                        break;
                    }

                    if (innerObject.ContainsKey(field))
                    {
                        innerObject = (ODataObject)innerObject[field];
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

        public void MapData(ref object obj)
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

        public static ODataObject ToDataObject<T>(T genericObject)
        {
            ODataObject dataObject = new ODataObject();
            Type genericObjectType = genericObject.GetType();

            // TODO: recursive mapping of nested objects
            foreach (PropertyInfo propertyInfo in genericObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string propertyName = propertyInfo.Name;
                bool isSerializable = true;
                OProperty oProperty = propertyInfo.GetCustomAttribute<OProperty>();

                if (oProperty != null)
                {
                    propertyName = oProperty.MappedTo;
                    isSerializable = oProperty.Serializable;
                }

                if (isSerializable)
                {
                    dataObject.Set(propertyName, propertyInfo.GetValue(genericObject, null));
                }

                /*if ((propertyInfo.PropertyType.IsArray || propertyInfo.PropertyType.IsGenericType))
                {
                    object propertyValue = propertyInfo.GetValue(propertyName);

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
                }*/
            }

            return dataObject;
        }
    }
}
