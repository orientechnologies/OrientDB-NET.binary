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
        private readonly Type _keyType;
        private readonly Type _valueType;

        public DictionaryFieldMapping(PropertyInfo propertyInfo, string fieldPath)
            : base(propertyInfo, fieldPath)
        {
            Type dictionaryType = propertyInfo.PropertyType;

            _keyType = propertyInfo.PropertyType.GetTypeInfo().GetGenericArguments()[0];
            _valueType = propertyInfo.PropertyType.GetTypeInfo().GetGenericArguments()[1];

            _needsMapping = !NeedsNoConversion(_valueType);
            if (_needsMapping)
            {
                _mapper = TypeMapperBase.GetInstanceFor(_valueType);
                _elementFactory = FastConstructor.BuildConstructor(_valueType);
            }

            if (propertyInfo.PropertyType.GetTypeInfo().IsInterface)
            {
                dictionaryType = typeof(List<>).MakeGenericType(_keyType);
            }

            _dictionaryFactory = FastConstructor.BuildConstructor<int>(dictionaryType);
        }

        protected override object CreateCollectionInstance(int collectionSize)
        {
            return _dictionaryFactory(collectionSize);
        }

        protected override void MapToNamedField(ODocument document, TTarget typedObject)
        {
            ODocument sourcePropertyValue = document.GetField<ODocument>(_fieldPath);
            var collection = CreateCollectionInstance(sourcePropertyValue.Count);

            AddItemToCollection(collection, 0, sourcePropertyValue);

            SetPropertyValue(typedObject, collection);
        }

        protected override void AddItemToCollection(object collection, int index, object item)
        {
            var enumerator = ((IDictionary)item).GetEnumerator();
            while (enumerator.MoveNext())
            {
                object key = enumerator.Key;
                object value = enumerator.Value;

                if (_keyType == typeof(Int16) ||
                    _keyType == typeof(Int32) ||
                    _keyType == typeof(Int64))
                {
                    key = Convert.ChangeType(enumerator.Key, _keyType);
                }
                else if (_keyType == typeof(Guid))
                {
                    key = Guid.Parse(enumerator.Key.ToString());
                }
                else if (_keyType.GetTypeInfo().IsEnum)
                {
                    key = Enum.Parse(_keyType, enumerator.Key.ToString());
                }
                if (_valueType == typeof(Int16) ||
                    _valueType == typeof(Int32) ||
                    _valueType == typeof(Int64))
                {
                    value = Convert.ChangeType(enumerator.Value, _valueType);
                }
                else if (_valueType == typeof(Guid))
                {
                    value = Guid.Parse(enumerator.Value.ToString());
                }
                else if (_valueType.GetTypeInfo().IsEnum)
                {
                    value = Enum.Parse(_valueType, enumerator.Value.ToString());
                }

                if (_needsMapping)
                {
                    var oMaped = _elementFactory();
                    _mapper.ToObject((ODocument)value, oMaped);
                    value = oMaped;
                }

                ((IDictionary)collection).Add(key, value);
            }
        }
        public override void MapToDocument(TTarget typedObject, ODocument document)
        {

            var dictionaryType = typeof(Dictionary<,>).MakeGenericType(_keyType, _needsMapping ? typeof(ODocument) : _valueType);

            var targetDictionary = (IDictionary)Activator.CreateInstance(dictionaryType);

            var sourceList = (IDictionary)GetPropertyValue(typedObject);

            if (sourceList != null)
            {
                var enumerator = sourceList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    targetDictionary.Add(enumerator.Key, _needsMapping ? _mapper.ToDocument(enumerator.Value) : enumerator.Value);
                }
            }

            document.SetField(_fieldPath, targetDictionary);
        }
    }
}
