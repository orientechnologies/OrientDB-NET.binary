using System;

namespace Orient.Client
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OProperty : Attribute
    {
        public string MapTo { get; set; }

        public OProperty(string mapTo)
        {
            MapTo = mapTo;
        }
    }
}
