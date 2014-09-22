using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Orient.Client.Mapping
{
    internal class CollectionNamedFieldMapping : NamedFieldMapping
    {
        private TypeMapperBase _mapper;
        private Type _targetElementType;
        private bool _needsMapping;

        public CollectionNamedFieldMapping(PropertyInfo propertyInfo, string fieldPath)
            : base(propertyInfo, fieldPath)
        {
            _targetElementType = GetTargetElementType();
            _needsMapping = !NeedsNoConversion(_targetElementType);
            if (_needsMapping)
                _mapper = TypeMapperBase.GetInstanceFor(_targetElementType);
        }

        protected override void MapToNamedField(ODocument document, object typedObject)
        {
            object sourcePropertyValue = document.GetField<object>(_fieldPath);

            IList collection = sourcePropertyValue as IList;
            if (collection == null) // if we only have one item currently stored (but scope for more) we create a temporary list and put our single item in it.
            {
                collection = new ArrayList();
                if (sourcePropertyValue != null)
                    collection.Add(sourcePropertyValue);
            }


            if (collection.Count > 0)
            {
                // create instance of property type
                object collectionInstance = Activator.CreateInstance(_propertyInfo.PropertyType, collection.Count);

                if (_propertyInfo.PropertyType.IsArray)
                {
                    if (!_needsMapping)
                    {
                        for (int i = 0; i < collection.Count; i++)
                        {
                            ((Array) collectionInstance).SetValue(collection[i], i);
                        }
                    }
                    else if (collection[0] is ODocument)
                    {
                        for (int i = 0; i < collection.Count; i++)
                        {
                            object element = Activator.CreateInstance(_targetElementType);
                            _mapper.ToObject((ODocument) collection[i], element);
                            ((Array) collectionInstance).SetValue(element, i);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                else if (_propertyInfo.PropertyType.IsGenericType)
                {
                    foreach (var t in collection)
                    {
                        // generic collection consists of basic types or ORIDs
                        if (!_needsMapping)
                        {
                            ((IList) collectionInstance).Add(t);
                        }
                            // generic collection consists of generic type which should be parsed
                        else
                        {
                            // create instance object based on first element of generic collection
                            object element = Activator.CreateInstance(_targetElementType);
                            _mapper.ToObject((ODocument)t, element);

                            ((IList)collectionInstance).Add(element);
                        }
                    }
                }
                else
                {
                    foreach (var t in collection)
                    {
                        object v = Activator.CreateInstance(t.GetType(), t);

                        ((IList) collectionInstance).Add(v);
                    }
                }

                _propertyInfo.SetValue(typedObject, collectionInstance, null);
            }
        }

        private Type GetTargetElementType()
        {
            if (_propertyInfo.PropertyType.IsArray)
                return _propertyInfo.PropertyType.GetElementType();
            if (_propertyInfo.PropertyType.IsGenericType)
                return _propertyInfo.PropertyType.GetGenericArguments().First();

            throw new NotImplementedException();

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