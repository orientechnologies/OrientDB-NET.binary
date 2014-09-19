using System;
using System.Reflection;

namespace Orient.Client.Mapping
{
    internal class ClassFieldMapping<T> : FieldMapping where T : new()
    {
        public ClassFieldMapping(PropertyInfo propertyInfo, string fieldPath) : base(propertyInfo, fieldPath)
        {
        }

        public override void MapToObject(ODocument document, object typedObject)
        {
            T result = new T();
            TypeMapper<T>.Instance.ToObject(document.GetField<ODocument>(_fieldPath), result);
            _propertyInfo.SetValue(typedObject, result, null);
        }
    }
}