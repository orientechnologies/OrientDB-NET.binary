using System;

namespace Orient.Client
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OProperty : Attribute
    {
        public string Alias { get; set; }
        public bool Serializable { get; set; }
        public bool Deserializable { get; set; }
        // TODO:
        //public OType Type { get; set; }

        public OProperty()
        {
            Alias = "";
            Serializable = true;
            Deserializable = true;
        }
    }
}
