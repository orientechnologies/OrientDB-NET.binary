using System;

namespace Orient.Client
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OProperty : Attribute
    {
        public string MapTo { get; set; }
        public OType Type { get; set; }
    }
}
