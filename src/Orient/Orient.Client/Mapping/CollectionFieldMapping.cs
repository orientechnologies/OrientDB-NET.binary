using System;
using System.Collections;
using System.Reflection;

namespace Orient.Client.Mapping
{
    internal class ListNamedFieldMapping<TTarget> : CollectionNamedFieldMapping<TTarget>
    {
        private readonly Func<object> _listFactory;

        public ListNamedFieldMapping(PropertyInfo propertyInfo, string fieldPath) : base(propertyInfo, fieldPath)
        {
            _listFactory = FastConstructor.BuildConstructor(_propertyInfo.PropertyType);
        }

        protected override object CreateCollectionInstance(int collectionSize)
        {
            return _listFactory();
        }

        protected override void AddItemToCollection(object collection, int index, object item)
        {
            ((IList) collection).Add(item);
        }
    }

    internal class ArrayNamedFieldMapping<TTarget> : CollectionNamedFieldMapping<TTarget>
    {
        private Func<int, object> _arrayFactory;

        public ArrayNamedFieldMapping(PropertyInfo propertyInfo, string fieldPath)
            : base(propertyInfo, fieldPath)
        {
            _arrayFactory = FastConstructor.BuildConstructor<int>(propertyInfo.PropertyType);
        }


        protected override object CreateCollectionInstance(int collectionSize)
        {
            return  _arrayFactory(collectionSize);
        }

        protected override void AddItemToCollection(object collection, int index, object item)
        {
            ((IList)collection)[index] = item;
        }
    }
}