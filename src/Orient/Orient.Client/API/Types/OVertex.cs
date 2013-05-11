using System.Collections.Generic;

namespace Orient.Client
{
    public class OVertex : ODocument
    {
        [OProperty(Alias = "in_", Serializable = false)]
        public HashSet<ORID> InE 
        {
            get
            {
                return this.GetField<HashSet<ORID>>("in_");
            }
        }

        [OProperty(Alias = "out_", Serializable = false)]
        public HashSet<ORID> OutE 
        {
            get
            {
                return this.GetField<HashSet<ORID>>("out_");
            }
        }

        public OVertex()
        {
            this.OClassName = "V";
        }
    }
}
