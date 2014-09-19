using System;
using System.Reflection;

namespace Orient.Client.Mapping
{
    internal class ClassNamedFieldMapping<T> : NamedFieldMapping
    {
        public ClassNamedFieldMapping(PropertyInfo propertyInfo, string fieldPath) : base(propertyInfo, fieldPath)
        {
        }

        protected override void MapToNamedField(ODocument document, object typedObject)
        {
            throw new NotImplementedException();
        }
    }
}