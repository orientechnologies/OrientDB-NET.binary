using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Orient.Client.Mapping
{
    class FastPropertyAccessor
    {
        public static Func<T, TReturn> BuildTypedGetter<T, TReturn>(PropertyInfo propertyInfo)
        {
            Func<T, TReturn> reflGet = (Func<T, TReturn>)
                Delegate.CreateDelegate(typeof(Func<T, TReturn>), propertyInfo.GetGetMethod());
            return reflGet;
        }

        public static Action<T, TProperty> BuildTypedSetter<T, TProperty>(PropertyInfo propertyInfo)
        {
            Action<T, TProperty> reflSet = (Action<T, TProperty>)Delegate.CreateDelegate(
                typeof(Action<T, TProperty>), propertyInfo.GetSetMethod());
            return reflSet;
        }

        public static Action<T, object> BuildUntypedSetter<T>(PropertyInfo propertyInfo)
        {
            var targetType = propertyInfo.DeclaringType;
            var methodInfo = propertyInfo.GetSetMethod();
            var exTarget = Expression.Parameter(targetType, "t");
            var exValue = Expression.Parameter(typeof(object), "p");
            
            // wir betreiben ein anObject.SetPropertyValue(object)
            var exBody = Expression.Call(exTarget, methodInfo,
                                       Expression.Convert(exValue, propertyInfo.PropertyType));
            var lambda = Expression.Lambda<Action<T, object>>(exBody, exTarget, exValue);
            // (t, p) => t.set_StringValue(Convert(p))

            var action = lambda.Compile();
            return action;
        }

        public static Func<T, object> BuildUntypedGetter<T>(PropertyInfo propertyInfo)
        {
            var targetType = propertyInfo.DeclaringType;
            var methodInfo = propertyInfo.GetGetMethod();
            var returnType = methodInfo.ReturnType;

            var exTarget = Expression.Parameter(targetType, "t");
            var exBody = Expression.Call(exTarget, methodInfo);
            var exBody2 = Expression.Convert(exBody, typeof(object));

            var lambda = Expression.Lambda<Func<T, object>>(exBody2, exTarget);
            // t => Convert(t.get_Foo())

            var action = lambda.Compile();
            return action;
        }
    }

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
            MapToObject(document, (TTarget) typedObject);
        }

        public void MapToDocument(object typedObject, ODocument document)
        {
            MapToDocument((TTarget) typedObject, document);
        }
    }

    internal abstract class NamedFieldMapping<TTarget> : FieldMapping<TTarget>
    {
        protected NamedFieldMapping(PropertyInfo propertyInfo, string fieldPath) : base(propertyInfo, fieldPath)
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