using System.Reflection;

namespace Orient.Client.Mapping
{
    internal class BasicNamedFieldMapping<TTarget> : NamedFieldMapping<TTarget>
    {
        public BasicNamedFieldMapping(PropertyInfo propertyInfo, string fieldPath) : base(propertyInfo, fieldPath)
        {
            
        }

        protected override void MapToNamedField(ODocument document, TTarget typedObject)
        {
            SetPropertyValue(typedObject, document.GetField<object>(_fieldPath));
        }

        public override void MapToDocument(TTarget typedObject, ODocument document)
        {
            object value = GetPropertyValue(typedObject);
            document.SetField(_fieldPath, value);
        }
    }
}