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
            PropertyInfo propertyInfo = mappingType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
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

            foreach (PropertyInfo propertyInfo in genericObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!propertyInfo.CanRead )
                    continue; // read only or write only properties can be ignored

                string propertyName = propertyInfo.Name;
                var propertyType = propertyInfo.PropertyType;

                object[] oProperties = propertyInfo.GetCustomAttributes(typeof(OProperty), true);

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
                    _fields.Add(new ORIDSimplePropertyUpdater(propertyInfo));

                if (propertyType.IsArray && propertyType.GetElementType() == ORIDType)
                    _fields.Add(new ORIDArrayPropertyUpdater(propertyInfo));

                if (propertyType.IsGenericType && propertyType.GetGenericArguments().First() == ORIDType)
                {
                    switch (propertyType.Name)
                    {
                        case "HashSet`1":
                            _fields.Add(new ORIDHashSetUpdater(propertyInfo));
                            break;
                        case "List`1":
                            _fields.Add(new ORIDListPropertyUpdater(propertyInfo));
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

    internal class ORIDListPropertyUpdater : ORIDPropertyUpdater<List<ORID>> 
    {
        public ORIDListPropertyUpdater(PropertyInfo propertyInfo) : base(propertyInfo)
        {
        }

        public override void Update(object oTarget, Dictionary<ORID, ORID> mappings)
        {
            var orids = GetValue(oTarget);
            if (orids == null)
                return;
            for (int i = 0; i < orids.Count; i++)
            {
                ORID replacement;
                if (mappings.TryGetValue(orids[i], out replacement))
                {
                    orids[i] = replacement;
                }
            }
        }
    }

    internal class ORIDHashSetUpdater : ORIDPropertyUpdater<HashSet<ORID>> 
    {
        public ORIDHashSetUpdater(PropertyInfo propertyInfo) : base(propertyInfo)
        {
        }

        public override void Update(object oTarget, Dictionary<ORID, ORID> mappings)
        {
            var orids = GetValue(oTarget);
            if (orids == null)
                return;
            foreach (var orid in orids.ToList())
            {
                ORID replacement;
                if (mappings.TryGetValue(orid, out replacement))
                {
                    orids.Remove(orid);
                    orids.Add(replacement);
                }
            }
        }
    }

    internal class ORIDSimplePropertyUpdater : ORIDPropertyUpdater<ORID>
    {
        public ORIDSimplePropertyUpdater(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            
        }

        public override void Update(object oTarget, Dictionary<ORID, ORID> mappings)
        {
            ORID orid = GetValue(oTarget);
            if (orid == null)
                return;
            ORID replacement;
            if (mappings.TryGetValue(orid, out replacement))
                SetValue(oTarget, replacement);

        }
    }

    internal class ORIDArrayPropertyUpdater : ORIDPropertyUpdater<ORID[]>
    {
        public ORIDArrayPropertyUpdater(PropertyInfo propertyInfo) : base(propertyInfo)
        {
        }

        public override void Update(object oTarget, Dictionary<ORID, ORID> mappings)
        {
            ORID[] orids = GetValue(oTarget);
            if (orids == null)
                return;
            for (int i = 0; i < orids.Length; i++)
            {
                ORID replacement;
                if (mappings.TryGetValue(orids[i], out replacement))
                {
                    orids[i] = replacement;
                }
            }
        }
    }

    internal abstract class ORIDPropertyUpdater<T> : IORIDPropertyUpdater
    {
        private readonly PropertyInfo _propertyInfo;

        protected ORIDPropertyUpdater(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        protected T GetValue(object oTarget)
        {
            return (T) _propertyInfo.GetValue(oTarget, null);
        }

        protected void SetValue(object oTarget, T value)
        {
            _propertyInfo.SetValue(oTarget, value, null );
        }

        public abstract void Update(object oTarget, Dictionary<ORID, ORID> mappings);
    }

    internal interface IORIDPropertyUpdater
    {
        void Update(object oTarget, Dictionary<ORID, ORID> mappings);
    }
}
