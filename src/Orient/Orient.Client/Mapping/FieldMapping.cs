using System;
using System.Reflection;

namespace Orient.Client.Mapping
{
    interface IFieldMapping
    {
        void MapToObject(ODocument document, object typedObject);
        void MapToDocument(object typedObject, ODocument document);
    }

    internal abstract class FieldMapping<TTarget> : IFieldMapping
    {
        protected PropertyInfo _propertyInfo;
        protected string _fieldPath;
        private Action<TTarget, object> _setter;
        private Func<TTarget, object> _getter;

        protected FieldMapping(PropertyInfo propertyInfo, string fieldPath)
        {
            if (propertyInfo != null)
            {
                _setter = FastPropertyAccessor.BuildUntypedSetter<TTarget>(propertyInfo);
                _getter = FastPropertyAccessor.BuildUntypedGetter<TTarget>(propertyInfo);
            }
            _propertyInfo = propertyInfo;
            _fieldPath = fieldPath;
        }

        protected object GetPropertyValue(TTarget target)
        {
            return _getter(target);
        }

        protected void SetPropertyValue(TTarget target, object value)
        {
                _setter(target, value);
        }


        public abstract void MapToObject(ODocument document, TTarget typedObject);
        public abstract void MapToDocument(TTarget typedObject, ODocument document);

        public void MapToObject(ODocument document, object typedObject)
        {
            MapToObject(document, (TTarget)typedObject);
        }

        public void MapToDocument(object typedObject, ODocument document)
        {
            MapToDocument((TTarget)typedObject, document);
        }
    }
    
    internal abstract class NamedFieldMapping<TTarget> : FieldMapping<TTarget>
    {
        protected NamedFieldMapping(PropertyInfo propertyInfo, string fieldPath)
            : base(propertyInfo, fieldPath)
        {
        }

        public override void MapToObject(ODocument document, TTarget typedObject)
        {
            if (document.HasField(_fieldPath))
                MapToNamedField(document, typedObject);
        }


        protected abstract void MapToNamedField(ODocument document, TTarget typedObject);
    }
}