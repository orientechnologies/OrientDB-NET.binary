using System.Collections.Generic;

namespace Orient.Client
{
    public class OGraphVertex
    {
        [OProperty(Alias = "in")]
        public List<ORID> In { get; set; }

        [OProperty(Alias = "out")]
        public List<ORID> Out { get; set; }

        public OGraphVertex()
        {
            In = new List<ORID>();
            Out = new List<ORID>();
        }
    }
}
