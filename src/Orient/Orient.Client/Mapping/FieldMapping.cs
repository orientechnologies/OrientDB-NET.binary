using System.Reflection;

namespace Orient.Client.Mapping
{
    internal abstract class FieldMapping
    {
        protected PropertyInfo _propertyInfo;
        protected string _fieldPath;

        protected FieldMapping(PropertyInfo propertyInfo, string fieldPath)
        {
            _propertyInfo = propertyInfo;
            _fieldPath = fieldPath;
        }

        public abstract void MapToObject(ODocument document, object typedObject);
    }

    internal abstract class NamedFieldMapping : FieldMapping
    {
        protected NamedFieldMapping(PropertyInfo propertyInfo, string fieldPath) : base(propertyInfo, fieldPath)
        {
        }

        public override void MapToObject(ODocument document, object typedObject)
        {
            if (document.HasField(_fieldPath))
                MapToNamedField(document, typedObject);
        }

        protected abstract void MapToNamedField(ODocument document, object typedObject);
    }
}