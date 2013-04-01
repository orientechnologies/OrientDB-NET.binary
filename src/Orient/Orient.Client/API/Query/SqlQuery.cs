using System.Collections;
using System.Collections.Generic;
using Orient.Client.Protocol;

namespace Orient.Client
{
    internal class SqlQuery
    {
        private string _value;

        internal bool HasSet { get; set; }

        internal SqlQuery()
        {
            _value = "";
            HasSet = false;
        }

        internal SqlQuery(string sql)
        {
            _value = sql;
            HasSet = false;
        }

        internal SqlQuery(params string[] values)
        {
            Join(values);
            HasSet = false;
        }

        internal void Surround(string prefix)
        {
            int lastSpaceIndex = _value.LastIndexOf(' ');

            if (lastSpaceIndex != -1)
            {
                _value = _value.Insert(lastSpaceIndex + 1, prefix + "(") + ")";
            }
        }

        internal void Join(params string[] values)
        {
            _value += string.Join(" ", values);
        }

        internal void SetField<T>(string fieldName, T fieldValue)
        {
            if (HasSet)
            {
                Join(", ");
            }
            else
            {
                Join("", Q.Set, "");
            }

            Join(fieldName, "=");

            if (fieldValue == null)
            {
                Join("", "null");
            }
            else if (fieldValue is string)
            {
                Join("", "'" + fieldValue + "'");
            }
            else if (fieldValue is IList)
            {
                Join("", "[");
                IList collection = (IList)fieldValue;
                int iteration = 0;

                foreach (object item in collection)
                {
                    Join(SqlQuery.ToString(item));

                    iteration++;

                    if (iteration < collection.Count)
                    {
                        Join(Q.Comma, "");
                    }
                }

                Join("]");
            }
            else
            {
                Join("", fieldValue.ToString());
            }

            if (!HasSet)
            {
                HasSet = true;
            }
        }

        internal void SetFields<T>(T obj)
        {
            ODataObject fields;

            if (obj is ODataObject)
            {
                fields = obj as ODataObject;
            }
            else
            {
                fields = ODataObject.ToDataObject<T>(obj);
            }

            // TODO: go also through embedded fields
            foreach (KeyValuePair<string, object> field in fields)
            {
                SetField(field.Key, field.Value);
            }
        }

        public override string ToString()
        {
            return _value;
        }

        internal static string ToString(object value)
        {
            string sql = "";

            if (value is string)
            {
                sql = string.Join(" ", "'" + value + "'");
            }
            else
            {
                sql = string.Join(" ", value.ToString());
            }

            return sql;
        }
    }
}
