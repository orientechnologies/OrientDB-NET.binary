using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Orient.Client.Mapping
{
    internal class NullableTimeSpanFieldMapping<TTarget> : BasicNamedFieldMapping<TTarget>
    {
        public NullableTimeSpanFieldMapping(PropertyInfo propertyInfo, string fieldPath)
            : base(propertyInfo, fieldPath)
        {

        }

        protected override void MapToNamedField(ODocument document, TTarget typedObject)
        {
            Nullable<TimeSpan> timespan = document.GetField<Nullable<TimeSpan>>(_fieldPath);

            SetPropertyValue(typedObject, timespan);
        }
    }
}
