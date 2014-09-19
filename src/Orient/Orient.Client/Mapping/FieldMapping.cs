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

        public abstract void MapToObject(ODocument document, object typedObject, string basePath);

        protected string FieldPath(string basePath)
        {
            return string.IsNullOrEmpty(basePath) ? _fieldPath :  basePath + "." + _fieldPath;
        }
    }

    internal abstract class NamedFieldMapping : FieldMapping
    {
        protected NamedFieldMapping(PropertyInfo propertyInfo, string fieldPath) : base(propertyInfo, fieldPath)
        {
        }

        public override void MapToObject(ODocument document, object typedObject, string basePath)
        {
            if (document.HasField(FieldPath(basePath)))
                MapToNamedField(document, typedObject, basePath);
        }

        protected abstract void MapToNamedField(ODocument document, object typedObject, string basePath);
    }
}