using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Orient.Client.Mapping
{
    internal class CollectionNamedFieldMapping : NamedFieldMapping
    {
        public CollectionNamedFieldMapping(PropertyInfo propertyInfo, string fieldPath)
            : base(propertyInfo, fieldPath)
        {
        }

        protected override void MapToNamedField(ODocument document, object typedObject)
        {
            object propertyValue = document.GetField<object>(_fieldPath);

            IList collection = propertyValue as IList;
            if (collection == null) // if we only have one item currently stored (but scope for more) we create a temporary list and put our single item in it.
            {
                collection = new ArrayList();
                if (propertyValue != null)
                    collection.Add(propertyValue);
            }


            if (collection.Count > 0)
            {
                // create instance of property type
                object collectionInstance = Activator.CreateInstance(_propertyInfo.PropertyType, collection.Count);

                if (_propertyInfo.PropertyType.IsArray)
                {
                    var elementType = _propertyInfo.PropertyType.GetElementType();
                    if (NeedsNoConversion(elementType))
                    {
                        for (int i = 0; i < collection.Count; i++)
                        {
                            ((Array) collectionInstance).SetValue(collection[i], i);
                        }
                    }
                    else
                    {
                        if (collection[0] is ODocument)
                        {
                            TypeMapperBase tmb = TypeMapperBase.GetInstanceFor(elementType);
                            for (int i = 0; i < collection.Count; i++)
                            {
                                object element = Activator.CreateInstance(elementType);
                                tmb.ToObject((ODocument) collection[i], element);
                                ((Array)collectionInstance).SetValue(element, i);
                            }
                        }
                        else
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    for (int i = 0; i < collection.Count; i++)
                    {
                        if (_propertyInfo.PropertyType.IsGenericType && (propertyValue is IEnumerable))
                        {
                            Type elementType = collection[i].GetType();

                            // generic collection consists of basic types or ORIDs
                            if (NeedsNoConversion(elementType))
                            {
                                ((IList) collectionInstance).Add(collection[i]);
                            }
                                // generic collection consists of generic type which should be parsed
                            else
                            {
                                // create instance object based on first element of generic collection
                                object instance = Activator.CreateInstance(_propertyInfo.PropertyType.GetGenericArguments().First(), null);



                                throw new NotImplementedException();
                                //                            ((IList) collectionInstance).Add(ToObject(instance, fieldPath));
                            }
                        }
                        else
                        {
                            object v = Activator.CreateInstance(collection[i].GetType(), collection[i]);

                            ((IList) collectionInstance).Add(v);
                        }
                    }
                }

                _propertyInfo.SetValue(typedObject, collectionInstance, null);
            }
        }

        private static bool NeedsNoConversion(Type elementType)
        {
            return elementType.IsPrimitive ||
                   (elementType == typeof (string)) ||
                   (elementType == typeof (DateTime)) ||
                   (elementType == typeof (decimal)) ||
                   (elementType == typeof (ORID));
        }
    }
}