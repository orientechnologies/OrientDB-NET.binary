using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Orient.Client.Mapping
{
    internal class ListNamedFieldMapping<TTarget> : CollectionNamedFieldMapping<TTarget>
    {
        private readonly Func<object> _listFactory;

        public ListNamedFieldMapping(PropertyInfo propertyInfo, string fieldPath)
            : base(propertyInfo, fieldPath)
        {
            Type listType = propertyInfo.PropertyType;
            if (propertyInfo.PropertyType.GetTypeInfo().IsInterface)
            {
                Type paramType = propertyInfo.PropertyType.GetTypeInfo().GetGenericArguments()[0];
                listType = typeof(List<>).MakeGenericType(paramType);
            }

            _listFactory = FastConstructor.BuildConstructor(listType);
        }

        protected override object CreateCollectionInstance(int collectionSize)
        {
            return _listFactory();
        }

        protected override void AddItemToCollection(object collection, int index, object item)
        {
            Type itemType = _propertyInfo.PropertyType.GetTypeInfo().GetGenericArguments()[0];
            if (itemType.GetTypeInfo().IsEnum)
            {
                ((IList)collection).Add(Enum.Parse(itemType, item.ToString()));
            }
            else if (itemType == typeof(Guid))
            {
                ((IList)collection).Add(Guid.Parse(item.ToString()));
            }
            else if (item is IConvertible || itemType.GetTypeInfo().IsAssignableFrom(item.GetType()))
            {
                ((IList)collection).Add(Convert.ChangeType(item, itemType));
            }
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
            return _arrayFactory(collectionSize);
        }

        protected override void AddItemToCollection(object collection, int index, object item)
        {
            Type itemType = _propertyInfo.PropertyType.GetElementType();
            if (itemType.GetTypeInfo().IsEnum)
            {
                ((IList)collection)[index] = Enum.Parse(itemType, item.ToString());
            }
            else if (itemType == typeof(Guid))
            {
                ((IList)collection)[index] = Guid.Parse(item.ToString());
            }
            else if (item == null)
            {
                ((IList)collection)[index] = null;
            }
            else if (item is IConvertible || itemType.GetTypeInfo().IsAssignableFrom(item.GetType()))
            {
                ((IList)collection)[index] = Convert.ChangeType(item, itemType);
            }
        }
    }
}
