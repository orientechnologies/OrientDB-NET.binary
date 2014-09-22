using System.Reflection;

namespace Orient.Client.Mapping
{
    internal class BasicNamedFieldMapping : NamedFieldMapping
    {
        public BasicNamedFieldMapping(PropertyInfo propertyInfo, string fieldPath) : base(propertyInfo, fieldPath)
        {
            
        }

        protected override void MapToNamedField(ODocument document, object typedObject)
        {
            _propertyInfo.SetValue(typedObject, document.GetField<object>(_fieldPath), null);
        }

        public override void MapToDocument(object typedObject, ODocument document)
        {
            object value = _propertyInfo.GetValue(typedObject, null);
            document.SetField(_fieldPath, value);
        }
    }
}