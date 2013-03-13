using System;

namespace Orient.Client
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OProperty : Attribute
    {
        public string MappedTo { get; set; }
        public OType Type { get; set; }
    }
}
