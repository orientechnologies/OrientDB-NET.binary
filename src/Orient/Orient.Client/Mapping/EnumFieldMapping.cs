using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Orient.Client.Mapping
{
    internal class EnumFieldMapping<TTarget> : BasicNamedFieldMapping<TTarget>
    {
        public EnumFieldMapping(PropertyInfo propertyInfo, string fieldPath)
            : base(propertyInfo, fieldPath)
        {

        }

        protected override void MapToNamedField(ODocument document, TTarget typedObject)
        {
            var value = Enum.Parse(_propertyInfo.PropertyType, document.GetField<string>(_fieldPath), true);
            SetPropertyValue(typedObject, value);
        }
    }
}
