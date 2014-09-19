using System.Reflection;

namespace Orient.Client.Mapping
{
    internal class BasicNamedFieldMapping : NamedFieldMapping
    {
        public BasicNamedFieldMapping(PropertyInfo propertyInfo, string fieldPath) : base(propertyInfo, fieldPath)
        {
            
        }

        protected override void MapToNamedField(ODocument document, object typedObject, string basePath)
        {
            _propertyInfo.SetValue(typedObject, document.GetField<object>(FieldPath(basePath)), null);
        }
    }
}