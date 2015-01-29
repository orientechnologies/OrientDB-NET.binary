using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Orient.Client.Mapping
{
    internal class GuidFieldMapping<TTarget> : BasicNamedFieldMapping<TTarget>
    {
        public GuidFieldMapping(PropertyInfo propertyInfo, string fieldPath)
            : base(propertyInfo, fieldPath)
        {

        }
        protected override void MapToNamedField(ODocument document, TTarget typedObject)
        {
            SetPropertyValue(typedObject, new Guid(document.GetField<string>(_fieldPath)));
        }
    }
}
