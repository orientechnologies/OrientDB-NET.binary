using System.Collections.Generic;
using System.Reflection;

namespace Orient.Client.Transactions
{
    internal class ORIDSimplePropertyUpdater<TTarget> : ORIDPropertyUpdater<TTarget, ORID>
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
}