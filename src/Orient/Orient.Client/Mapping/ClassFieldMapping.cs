using System;
using System.Reflection;

namespace Orient.Client.Mapping
{
    internal class ClassFieldMapping<TProperty, TTarget> : FieldMapping<TTarget> where TProperty : new()
    {
        public ClassFieldMapping(PropertyInfo propertyInfo, string fieldPath) : base(propertyInfo, fieldPath)
        {
        }

        public override void MapToObject(ODocument document, TTarget typedObject)
        {
            var documentField = document.GetField<ODocument>(_fieldPath);

            if (documentField == null)
            {
                return;
            }

            TProperty result = new TProperty();
            TypeMapper<TProperty>.Instance.ToObject(document.GetField<ODocument>(_fieldPath), result);
            SetPropertyValue(typedObject, result);
        }

        public override void MapToDocument(TTarget typedObject, ODocument document)
        {
            var sourceValue = GetPropertyValue(typedObject);

            ODocument subDoc = null;

            if (sourceValue != null)
            {
                subDoc = TypeMapper<TProperty>.Instance.ToDocument(sourceValue);
            }

            document.SetField(_fieldPath, subDoc);
        }
    }
}