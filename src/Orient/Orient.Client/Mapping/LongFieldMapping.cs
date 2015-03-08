using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Orient.Client.Mapping
{
    internal class LongFieldMapping<TTarget> : BasicNamedFieldMapping<TTarget>
    {
        public LongFieldMapping(PropertyInfo propertyInfo, string fieldPath)
            : base(propertyInfo, fieldPath)
        {

        }
        protected override void MapToNamedField(ODocument document, TTarget typedObject)
        {
            object item = document.GetField<object>(_fieldPath);
            if (item is IConvertible)
                SetPropertyValue(typedObject, Convert.ChangeType(item, typeof(long)));
        }
    }
}
