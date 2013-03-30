using System.Collections.Generic;
using Orient.Client.Protocol;

namespace Orient.Client
{
    internal class SqlQuery
    {
        internal string Value { get; set; }
        internal bool HasSet { get; set; }

        internal SqlQuery()
        {
            Value = "";
            HasSet = false;
        }

        internal SqlQuery(string sql)
        {
            Value = sql;
            HasSet = false;
        }

        internal SqlQuery(params string[] values)
        {
            Join(values);
            HasSet = false;
        }

        internal void Join(params string[] values)
        {
            Value += string.Join(" ", values);
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
    }
}
