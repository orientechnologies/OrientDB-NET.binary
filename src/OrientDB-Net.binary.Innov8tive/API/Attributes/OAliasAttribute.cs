using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orient.Client.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class OAliasAttribute : Attribute
    {

        public OAliasAttribute(string name)
        {
            this.Name = name;
        }

        public String Name { get; private set; }

    }
}
