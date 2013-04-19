using System.Collections;
using System.Collections.Generic;
using Orient.Client.Protocol;

namespace Orient.Client
{
    internal class SqlQuery
    {
        private string _value;

        internal bool HasSet { get; set; }
        internal bool HasAdd { get; set; }

        internal SqlQuery()
        {
            _value = "";
        }

        internal SqlQuery(string sql)
        {
            _value = sql;
        }

        internal SqlQuery(params string[] values)
        {
            Join(values);
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

        #region Where with conditions

        internal void Where(string field)
        {
            Join("", Q.Where, field);
        }

        internal void And(string field)
        {
            Join("", Q.And, field);
        }

        internal void Or(string field)
        {
            Join("", Q.Or, field);
        }

        internal void Equals<T>(T item)
        {
            Join("", Q.Equals, SqlQuery.ToString(item));
        }

        internal void NotEquals<T>(T item)
        {
            Join("", Q.NotEquals, SqlQuery.ToString(item));
        }

        internal void Lesser<T>(T item)
        {
            Join("", Q.Lesser, SqlQuery.ToString(item));
        }

        internal void LesserEqual<T>(T item)
        {
            Join("", Q.LesserEqual, SqlQuery.ToString(item));
        }

        internal void Greater<T>(T item)
        {
            Join("", Q.Greater, SqlQuery.ToString(item));
        }

        internal void GreaterEqual<T>(T item)
        {
            Join("", Q.GreaterEqual, SqlQuery.ToString(item));
        }

        internal void Like<T>(T item)
        {
            Join("", Q.Like, SqlQuery.ToString(item));
        }

        internal void IsNull()
        {
            Join("", Q.Is, Q.Null);
        }

        internal void Contains<T>(T item)
        {
            Join("", Q.Contains, SqlQuery.ToString(item));
        }

        internal void Contains<T>(string field, T value)
        {
            Join("", Q.Contains, "(" + field, Q.Equals, SqlQuery.ToString(value) + ")");
        }

        #endregion

        #region SetField(s)

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
            ODocument document;

            if (obj is ODocument)
            {
                document = obj as ODocument;
            }
            else
            {
                document = ODocument.ToDocument<T>(obj);
            }

            // TODO: go also through embedded fields
            foreach (KeyValuePair<string, object> field in document)
            {
                // set only fields which doesn't start with @ character
                if ((field.Key.Length > 0) && (field.Key[0] != '@'))
                {
                    SetField(field.Key, field.Value);
                }
            }
        }

        #endregion

        #region ToString

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

        #endregion
    }
}
