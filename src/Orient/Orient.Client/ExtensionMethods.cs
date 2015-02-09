using System;
using System.Collections.Generic;
using System.Globalization;
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

        public static string ToInvarianCultureString(this object value)
        {
            var formattable = value as IFormattable;
            if (value is float)
                return ((float)value).ToString("R", CultureInfo.InvariantCulture);
            else if (value is double)
                return ((double)value).ToString("R", CultureInfo.InvariantCulture);
            else
                return (formattable != null) ? formattable.ToString(null, CultureInfo.InvariantCulture) : value.ToString();
        }
    }
}
