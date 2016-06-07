using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Orient.Client.Mapping
{
    internal class DateTimeFieldMapping<TTarget> : BasicNamedFieldMapping<TTarget>
    {
        public DateTimeFieldMapping(PropertyInfo propertyInfo, string fieldPath)
            : base(propertyInfo, fieldPath)
        {

        }

        protected override void MapToNamedField(ODocument document, TTarget typedObject)
        {
            DateTime dateTime = document.GetField<DateTime>(_fieldPath);

            SetPropertyValue(typedObject, dateTime);
        }
    }
}
