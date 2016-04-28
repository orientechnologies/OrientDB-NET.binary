using System;
using System.Reflection;

namespace Orient.Client
{
    public class OEdge : ODocument
    {
        [OProperty(Alias = "in", Serializable = false)]
        public ORID InV
        {
            get
            {
                return this.GetField<ORID>("in");
            }
        }

        [OProperty(Alias = "out", Serializable = false)]
        public ORID OutV
        {
            get
            {
                return this.GetField<ORID>("out");
            }
        }

        [OProperty(Alias = "label", Serializable = false)]
        public string Label 
        {
            get
            {
                string label = this.GetField<string>("@OClassName");

                if (string.IsNullOrEmpty(label))
                {
                    return this.GetType().Name;
                }
                else
                {
                    return label;
                }
            }
        }
    }
}
