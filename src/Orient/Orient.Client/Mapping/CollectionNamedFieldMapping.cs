using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Orient.Client.Mapping
{
    internal abstract class CollectionNamedFieldMapping<TTarget> : NamedFieldMapping<TTarget>
    {
        protected TypeMapperBase _mapper;
        private readonly Type _targetElementType;
        protected bool _needsMapping;
        protected Func<object> _elementFactory;

        public CollectionNamedFieldMapping(PropertyInfo propertyInfo, string fieldPath)
            : base(propertyInfo, fieldPath)
        {
            _targetElementType = GetTargetElementType();
            _needsMapping = !NeedsNoConversion(_targetElementType);
            if (_needsMapping)
            {
                _mapper = TypeMapperBase.GetInstanceFor(_targetElementType);
                _elementFactory = FastConstructor.BuildConstructor(_targetElementType);
            }
        }

        protected abstract object CreateCollectionInstance(int collectionSize);
        protected abstract void AddItemToCollection(object collection, int index, object item);

        protected override void MapToNamedField(ODocument document, TTarget typedObject)
        {

            object sourcePropertyValue = document.GetField<object>(_fieldPath);

            var collection = sourcePropertyValue as IList;

            if (collection == null) // if we only have one item currently stored (but scope for more) we create a temporary list and put our single item in it.
            {
                collection = new ArrayList();
                if (sourcePropertyValue != null)
                {
                    // TODO: Implement in derived class due Different collection mapings
                    if (typeof(HashSet<Object>).IsAssignableFrom(sourcePropertyValue.GetType()))
                    {
                        foreach (var item in (HashSet<Object>)sourcePropertyValue)
                        {
                            collection.Add(item);
                        }
                    }
                    else
                    {
                        collection.Add(sourcePropertyValue);
                    }
                }
            }

            // create instance of property type
            var collectionInstance = CreateCollectionInstance(collection.Count);

            for (int i = 0; i < collection.Count; i++)
            {
                var t = collection[i];
                object oMapped = t;
                if (_needsMapping)
                {
                    try
                    {
                        object element = _elementFactory();

                        _mapper.ToObject((ODocument)t, element);
                        oMapped = element;
                    }
                    catch
                    {
                        // FIX: somtimes collection of embeded documents returned as ORID Collection;
                    }
                }

                AddItemToCollection(collectionInstance, i, oMapped);
            }

            SetPropertyValue(typedObject, collectionInstance);
        }

        private Type GetTargetElementType()
        {
            if (_propertyInfo.PropertyType.IsArray)
                return _propertyInfo.PropertyType.GetElementType();
            if (_propertyInfo.PropertyType.IsGenericType)
                return _propertyInfo.PropertyType.GetGenericArguments().First();

            throw new NotImplementedException();

        }

        protected bool NeedsNoConversion(Type elementType)
        {
            return elementType.IsPrimitive ||
                   (elementType == typeof(string)) ||
                   (elementType == typeof(DateTime)) ||
                   (elementType == typeof(decimal)) ||
                   (elementType == typeof(ORID)) ||
                   (elementType.IsValueType);
        }

        public override void MapToDocument(TTarget typedObject, ODocument document)
        {
            var targetElementType = _needsMapping ? typeof(ODocument) : _targetElementType;
            var listType = typeof(List<>).MakeGenericType(targetElementType);
            IList targetList = null;

            var sourceList = (IEnumerable)GetPropertyValue(typedObject);
            if (sourceList != null)
            {
                targetList = (IList)Activator.CreateInstance(listType);

                foreach (var item in sourceList)
                    targetList.Add(_needsMapping ? _mapper.ToDocument(item) : item);
            }

            document.SetField(_fieldPath, targetList);
        }
    }
}