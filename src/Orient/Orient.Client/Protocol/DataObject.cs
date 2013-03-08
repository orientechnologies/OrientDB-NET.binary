using System.Collections.Generic;

namespace Orient.Client.Protocol
{
    internal class DataObject : Dictionary<string, object>
    {
        internal T Get<T>(string fieldPath) where T : new()
        {
            T value = new T();

            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                DataObject innerObject = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        value = (T)innerObject[field];
                        break;
                    }

                    if (innerObject.ContainsKey(field))
                    {
                        innerObject = (DataObject)innerObject[field];
                        iteration++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if (this.ContainsKey(fieldPath))
                {
                    value = (T)this[fieldPath];
                }
            }

            return value;
        }

        internal void Set<T>(string fieldPath, T value)
        {
            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                DataObject innerObject = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        if (innerObject.ContainsKey(field))
                        {
                            innerObject[field] = value;
                        }
                        else
                        {
                            innerObject.Add(field, value);
                        }
                        break;
                    }

                    if (innerObject.ContainsKey(field))
                    {
                        innerObject = (DataObject)innerObject[field];
                        iteration++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if (this.ContainsKey(fieldPath))
                {
                    this[fieldPath] = value;
                }
                else
                {
                    this.Add(fieldPath, value);
                }
            }
        }

        internal bool Has(string fieldPath)
        {
            bool contains = false;

            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                DataObject innerObject = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        contains = innerObject.ContainsKey(field);
                        break;
                    }

                    if (innerObject.ContainsKey(field))
                    {
                        innerObject = (DataObject)innerObject[field];
                        iteration++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                contains = this.ContainsKey(fieldPath);
            }

            return contains;
        }
    }
}
