using System.Collections.Generic;

namespace Orient.Client
{
    public class OGraphVertex
    {
        [OProperty(MappedTo = "in")]
        public List<ORID> In { get; set; }

        [OProperty(MappedTo = "out")]
        public List<ORID> Out { get; set; }

        public OGraphVertex()
        {
            In = new List<ORID>();
            Out = new List<ORID>();
        }
    }
}
