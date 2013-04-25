using System.Collections.Generic;

namespace Orient.Client.Protocol
{
    internal class QueryCompiler : Dictionary<string, string>
    {
        // add/append key values with prefix space
        internal void Append(string key, params string[] values)
        {
            if (this.ContainsKey(key))
            {
                this[key] += string.Join(" ", values);
            }
            else
            {
                this.Add(key, string.Join(" ", values));
            }
        }

        // add/overwrite key values
        internal void Unique(string key, params string[] values)
        {
            if (this.ContainsKey(key))
            {
                this[key] = string.Join(" ", values);
            }
            else
            {
                this.Add(key, string.Join(" ", values));
            }
        }

        internal bool HasKey(string key)
        {
            return this.ContainsKey(key);
        }

        // return key value if the specified key is present
        internal string Value(string key)
        {
            if (!this.ContainsKey(key))
            {
                return "";
            }

            return this[key];
        }

        // return key value from the given order of keyword where the last item has the highest priority
        internal string OrderedValue(params string[] order)
        {
            string value = "";

            foreach (string keyword in order)
            {
                string keyValue = Value(keyword);

                if (!string.IsNullOrEmpty(keyValue))
                {
                    value = keyValue;
                }
            }

            return value;
        }
    }
}
