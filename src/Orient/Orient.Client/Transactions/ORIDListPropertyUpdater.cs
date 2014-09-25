using System.Collections.Generic;
using System.Reflection;

namespace Orient.Client.Transactions
{
    internal class ORIDListPropertyUpdater<TTarget> : ORIDPropertyUpdater<TTarget, List<ORID>> 
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
}