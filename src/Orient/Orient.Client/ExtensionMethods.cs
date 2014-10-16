using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Orient.Client
{
    static class ExtensionMethods
    {
        public static OProperty GetOPropertyAttribute(this PropertyInfo property)
        {
            return property.GetCustomAttributes(typeof(OProperty), true).OfType<OProperty>().FirstOrDefault();
        }
    }
}
