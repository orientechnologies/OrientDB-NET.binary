using System.Collections.Generic;

namespace Orient.Client
{
    public class OVertex : OBaseRecord
    {
        [OProperty(Alias = "in")]
        public HashSet<ORID> In { get; set; }

        [OProperty(Alias = "out")]
        public HashSet<ORID> Out { get; set; }

        public OVertex()
        {
            In = new HashSet<ORID>();
            Out = new HashSet<ORID>();
        }
    }
}
