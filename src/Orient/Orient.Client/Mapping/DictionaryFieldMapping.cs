using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Orient.Client.Mapping
{
    internal class DictionaryFieldMapping<TTarget> : CollectionNamedFieldMapping<TTarget>
    {
        private readonly Func<int, object> _dictionaryFactory;

        public DictionaryFieldMapping(PropertyInfo propertyInfo, string fieldPath)
            : base(propertyInfo, fieldPath)
        {
            Type dictionaryType = propertyInfo.PropertyType;
            if (propertyInfo.PropertyType.IsInterface)
            {
                Type paramType = propertyInfo.PropertyType.GetGenericArguments()[0];
                dictionaryType = typeof(List<>).MakeGenericType(paramType);
            }

            _dictionaryFactory = FastConstructor.BuildConstructor<int>(dictionaryType);
        }

        protected override object CreateCollectionInstance(int collectionSize)
        {
            return _dictionaryFactory(collectionSize);
        }

        protected override void AddItemToCollection(object collection, int index, object item)
        {
            var enumerator = ((IDictionary)item).GetEnumerator();
            while (enumerator.MoveNext())
            {
                ((IDictionary)collection).Add(enumerator.Key, enumerator.Value);
            }
        }
        public override void MapToDocument(TTarget typedObject, ODocument document)
        {
            var keyType = _propertyInfo.PropertyType.GetGenericArguments()[0];
            var valueType = _propertyInfo.PropertyType.GetGenericArguments()[1];

            var dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);

            var targetDictionary = (IDictionary)Activator.CreateInstance(dictionaryType);

            var sourceList = (IDictionary)GetPropertyValue(typedObject);

            if (sourceList != null)
            {
                var enumerator = sourceList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    targetDictionary.Add(enumerator.Key, enumerator.Value);
                }
            }

            document.SetField(_fieldPath, targetDictionary);
        }
    }
}
