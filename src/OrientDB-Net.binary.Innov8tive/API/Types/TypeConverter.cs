using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Orient.Client.API.Types
{
    internal class TypeConverter
    {
        static TypeConverter()
        {
            AddType<int>(OType.Integer);
            AddType<long>(OType.Long);
            AddType<short>(OType.Short);
            AddType<string>(OType.String);
            AddType<bool>(OType.Boolean);
            AddType<float>(OType.Float);
            AddType<double>(OType.Double);
            AddType<DateTime>(OType.DateTime);
            AddType<byte[]>(OType.Binary);
            AddType<byte>(OType.Byte);
            AddType<decimal>(OType.Decimal);
            AddType<int?>(OType.Integer);
            AddType<long?>(OType.Long);
            AddType<short?>(OType.Short);
            AddType<bool?>(OType.Boolean);
            AddType<float?>(OType.Float);
            AddType<double?>(OType.Double);
            AddType<DateTime?>(OType.DateTime);
            AddType<byte?>(OType.Byte);
            AddType<decimal?>(OType.Decimal);
            AddType<TimeSpan>(OType.String);
            AddType<TimeSpan?>(OType.String);
            AddType<HashSet<ORID>>(OType.LinkSet);
            AddType<List<ORID>>(OType.LinkList);
            AddType<ORID>(OType.Link);
        }

        private static void AddType<T>(OType name)
        {
            _types.Add(typeof(T), name);
        }

        static Dictionary<Type, OType> _types = new Dictionary<Type, OType>();

        public static OType TypeToDbName(Type t)
        {
            OType result;
            if (_types.TryGetValue(t, out result))
                return result;

            else if (t.FullName.Contains("System.Collections.Generic.HashSet"))
                return OType.EmbeddedSet;

            else if (typeof(System.Collections.IDictionary).GetTypeInfo().IsAssignableFrom(t))
                return OType.EmbeddedMap;

            else if (typeof(System.Collections.IEnumerable).GetTypeInfo().IsAssignableFrom(t))
                return OType.EmbeddedList;
            else if (!t.GetTypeInfo().IsPrimitive)
                return OType.Embedded;

            throw new ArgumentException("propertyType " + t.Name + " is not yet supported.");
        }
    }

}
