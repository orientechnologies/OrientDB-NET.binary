using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Orient.Client.Mapping
{
    internal class HashSetNamedFieldMapping<TTarget> : CollectionNamedFieldMapping<TTarget>
    {
        private readonly Func<object> _listFactory;
        private Action<object, object> _addFunc;

        public HashSetNamedFieldMapping(PropertyInfo propertyInfo, string fieldPath)
            : base(propertyInfo, fieldPath)
        {
            _listFactory = FastConstructor.BuildConstructor(_propertyInfo.PropertyType);
            _addFunc = FastCall.BuildCaller(_propertyInfo.PropertyType.GetTypeInfo().GetMethod("Add"));
        }

        protected override object CreateCollectionInstance(int collectionSize)
        {
            return _listFactory();
        }

        protected override void AddItemToCollection(object collection, int index, object item)
        {
            _addFunc(collection, item);
        }
    }
}