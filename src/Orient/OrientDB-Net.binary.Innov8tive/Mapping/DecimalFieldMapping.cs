using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Orient.Client.Mapping
{
    internal class DecimalFieldMapping<TTarget> : BasicNamedFieldMapping<TTarget>
    {
        public DecimalFieldMapping(PropertyInfo propertyInfo, string fieldPath)
            : base(propertyInfo, fieldPath)
        {

        }
        protected override void MapToNamedField(ODocument document, TTarget typedObject)
        {
            // Only until bug #3483 will be fixed than use decimal
            object item = document.GetField<object>(_fieldPath);
            if (item is IConvertible)
                SetPropertyValue(typedObject, Convert.ChangeType(item, typeof(Decimal)));
        }

    }
}
