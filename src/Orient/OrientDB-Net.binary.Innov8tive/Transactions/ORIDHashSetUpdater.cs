using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Orient.Client.Transactions
{
    internal class ORIDHashSetUpdater<TTarget> : ORIDPropertyUpdater<TTarget, HashSet<ORID>> 
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
}