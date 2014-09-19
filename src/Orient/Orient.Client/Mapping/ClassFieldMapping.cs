using System;
using System.Reflection;

namespace Orient.Client.Mapping
{
    internal class ClassFieldMapping<T> : FieldMapping where T : new()
    {
        public ClassFieldMapping(PropertyInfo propertyInfo, string fieldPath) : base(propertyInfo, fieldPath)
        {
        }

        public override void MapToObject(ODocument document, object typedObject, string basePath)
        {
            T result = new T();
            TypeMapper<T>.Instance.ToObject(document, result, FieldPath(basePath));
            _propertyInfo.SetValue(typedObject, result, null);
        }
    }
}