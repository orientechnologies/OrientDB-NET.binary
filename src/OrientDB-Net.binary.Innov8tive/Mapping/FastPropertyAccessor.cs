using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Orient.Client.Mapping
{
    class FastConstructor
    {
        public static Func<object> BuildConstructor(Type type)
        {
            Expression body = Expression.New(type);
            return Expression.Lambda<Func<object>>(body).Compile();    
        }

        public static Func<T, object> BuildConstructor<T>(Type type)
        {
            var param1 = Expression.Parameter(typeof(T), "param1");
            ConstructorInfo constructorInfo = type.GetTypeInfo().GetConstructor(new Type[] {typeof (T)});
            if (constructorInfo == null)
                throw new MissingMethodException(type.Name + " has so public constructore with 1 parameter of type " + typeof(T).Name);
            Expression body = Expression.New(constructorInfo, param1);
            return Expression.Lambda<Func<T, object>>(body, param1).Compile();    
            
        }

    }

    class FastPropertyAccessor
    {


        public static Func<T, TReturn> BuildTypedGetter<T, TReturn>(PropertyInfo propertyInfo)
        {
            return (Func<T, TReturn>)propertyInfo.GetGetMethod().CreateDelegate(typeof(Func<T, TReturn>));
        }

        public static Action<T, TProperty> BuildTypedSetter<T, TProperty>(PropertyInfo propertyInfo)
        {
            return (Action<T, TProperty>)propertyInfo.GetSetMethod().CreateDelegate(typeof(Action<T, TProperty>));
        }

        public static Action<T, object> BuildUntypedSetter<T>(PropertyInfo propertyInfo)
        {
            var targetType = propertyInfo.DeclaringType;
            var methodInfo = propertyInfo.GetSetMethod(true);
            var exTarget = Expression.Parameter(targetType, "t");
            var exValue = Expression.Parameter(typeof(object), "p");
            var exBody = Expression.Call(exTarget, methodInfo, Expression.Convert(exValue, propertyInfo.PropertyType));
            var lambda = Expression.Lambda<Action<T, object>>(exBody, exTarget, exValue);

            return lambda.Compile();
        }

        public static Func<T, object> BuildUntypedGetter<T>(PropertyInfo propertyInfo)
        {
            var targetType = propertyInfo.DeclaringType;
            var methodInfo = propertyInfo.GetGetMethod(true);

            var exTarget = Expression.Parameter(targetType, "t");
            var exBody = Expression.Call(exTarget, methodInfo);
            var exBody2 = Expression.Convert(exBody, typeof(object));

            var lambda = Expression.Lambda<Func<T, object>>(exBody2, exTarget);
            return lambda.Compile();
        }
    }

    class FastCall
    {
       

        public static Action<object, object> BuildCaller(MethodInfo methodInfo)
        {
            var targetType = methodInfo.DeclaringType;
            var param1Type = methodInfo.GetParameters()[0].ParameterType;
            var exTarget = Expression.Parameter(typeof(object), "t");
            var exValue = Expression.Parameter(typeof(object), "p");
            var exBody = Expression.Call(Expression.Convert(exTarget, targetType), methodInfo, Expression.Convert(exValue, param1Type));
            var lambda = Expression.Lambda<Action<object, object>>(exBody, exTarget, exValue);

            return lambda.Compile();
        }

    }
}