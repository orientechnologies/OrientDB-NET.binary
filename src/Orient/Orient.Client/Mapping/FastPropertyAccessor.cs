using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Orient.Client.Mapping
{
    class FastPropertyAccessor
    {
        public static Func<T, TReturn> BuildTypedGetter<T, TReturn>(PropertyInfo propertyInfo)
        {
            return (Func<T, TReturn>) Delegate.CreateDelegate(typeof(Func<T, TReturn>), propertyInfo.GetGetMethod());
        }

        public static Action<T, TProperty> BuildTypedSetter<T, TProperty>(PropertyInfo propertyInfo)
        {
            return (Action<T, TProperty>)Delegate.CreateDelegate(typeof(Action<T, TProperty>), propertyInfo.GetSetMethod());
        }

        public static Action<T, object> BuildUntypedSetter<T>(PropertyInfo propertyInfo)
        {
            var targetType = propertyInfo.DeclaringType;
            var methodInfo = propertyInfo.GetSetMethod();
            var exTarget = Expression.Parameter(targetType, "t");
            var exValue = Expression.Parameter(typeof(object), "p");
            var exBody = Expression.Call(exTarget, methodInfo, Expression.Convert(exValue, propertyInfo.PropertyType));
            var lambda = Expression.Lambda<Action<T, object>>(exBody, exTarget, exValue);

            return lambda.Compile();
        }

        public static Func<T, object> BuildUntypedGetter<T>(PropertyInfo propertyInfo)
        {
            var targetType = propertyInfo.DeclaringType;
            var methodInfo = propertyInfo.GetGetMethod();

            var exTarget = Expression.Parameter(targetType, "t");
            var exBody = Expression.Call(exTarget, methodInfo);
            var exBody2 = Expression.Convert(exBody, typeof(object));

            var lambda = Expression.Lambda<Func<T, object>>(exBody2, exTarget);
            return lambda.Compile();
        }
    }
}