using System;

namespace Orient.Client
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OProperty : Attribute
    {
        public string MappedTo { get; set; }
        public bool Serializable { get; set; }
        public bool Deserializable { get; set; }
        public OType Type { get; set; }

        public OProperty()
        {
            MappedTo = "";
            Serializable = true;
            Deserializable = true;
        }
    }
}
