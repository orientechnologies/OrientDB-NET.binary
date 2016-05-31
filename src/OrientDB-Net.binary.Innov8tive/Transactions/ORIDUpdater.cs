using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Orient.Client.Transactions
{
    internal abstract class ORIDUpdaterBase
    {
        protected static Type ORIDType = typeof (ORID);

        public abstract void UpdateORIDs(object oTarget, Dictionary<ORID, ORID> replacements);

        public static ORIDUpdaterBase GetInstanceFor(Type t)
        {
            var mappingType = typeof(ORIDUpdater<>).MakeGenericType(t);
            PropertyInfo propertyInfo = mappingType.GetTypeInfo().GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
            return (ORIDUpdaterBase)propertyInfo.GetValue(null, null);
        }
    }


    class ORIDUpdater<T> : ORIDUpdaterBase
    {
        private static readonly ORIDUpdater<T> _instance = new ORIDUpdater<T>();
        public static ORIDUpdater<T> Instance { get { return _instance; } }

        readonly List<IORIDPropertyUpdater> _fields = new List<IORIDPropertyUpdater>();

        private ORIDUpdater()
        {
            Type genericObjectType = typeof(T);

            foreach (PropertyInfo propertyInfo in genericObjectType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!propertyInfo.CanRead )
                    continue; // read only or write only properties can be ignored

                string propertyName = propertyInfo.Name;
                var propertyType = propertyInfo.PropertyType;

                object[] oProperties = propertyInfo.GetCustomAttributes(typeof(OProperty), true).ToArray();

                if (oProperties.Any())
                {
                    OProperty oProperty = oProperties.First() as OProperty;
                    if (oProperty != null)
                    {
                        if (!oProperty.Deserializable)
                            continue;
                        propertyName = oProperty.Alias;
                    }
                }

                if (propertyType == ORIDType && propertyInfo.CanWrite)
                    _fields.Add(new ORIDSimplePropertyUpdater<T>(propertyInfo));

                if (propertyType.IsArray && propertyType.GetElementType() == ORIDType)
                    _fields.Add(new ORIDArrayPropertyUpdater<T>(propertyInfo));

                if (propertyType.GetTypeInfo().IsGenericType && propertyType.GetTypeInfo().GetGenericArguments().First() == ORIDType)
                {
                    switch (propertyType.Name)
                    {
                        case "HashSet`1":
                            _fields.Add(new ORIDHashSetUpdater<T>(propertyInfo));
                            break;
                        case "List`1":
                            _fields.Add(new ORIDListPropertyUpdater<T>(propertyInfo));
                            break;
                        default:
                            throw new NotImplementedException("Generic ORID collection not handled.");
                    }
                    
                }

            }
        }

        public override void UpdateORIDs(object oTarget,  Dictionary<ORID, ORID> replacements)
        {
            foreach (var field in _fields)
                field.Update(oTarget, replacements);
        }
    }
}
