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
            DateTime dateTime;
            if (DateTime.TryParse(document.GetField<string>(_fieldPath), out dateTime))
            {

                SetPropertyValue(typedObject, dateTime);
            }
            else
            {
                throw new OException(OExceptionType.Deserialization, "Can't parse DateTime value " + typedObject);
            }
        }
    }
}
