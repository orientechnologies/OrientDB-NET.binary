using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Orient.Client.Protocol
{
    using System.Text.RegularExpressions;

    internal class SqlQuery
    {
        private QueryCompiler _compiler = new QueryCompiler();
        private Connection _connection;
        public SqlQuery(Connection connection)
        {
            _connection = connection;
        }
        internal void Class(string className)
        {
            _compiler.Unique(Q.Class, ParseClassName(className));
        }

        internal void Property(string propertyName, OType type)
        {
            _compiler.Unique(Q.Property, propertyName, type.ToString());
        }

        internal void LinkedType(OType type)
        {
            _compiler.Unique(Q.LinkedType, type.ToString());
        }

        internal void LinkedClass(string @class)
        {
            _compiler.Unique(Q.LinkedClass, @class);
        }

        #region Cluster

        internal void Cluster(string clusterName)
        {
            _compiler.Unique(Q.Cluster, ParseClassName(clusterName));
        }

        internal void Cluster(string clusterName, OClusterType clustertype)
        {
            _compiler.Unique(Q.Cluster, ParseClassName(clusterName), clustertype.ToString().ToUpper());
        }

        #endregion

        internal void Record(ORID orid)
        {
            _compiler.Unique(Q.Record, orid.ToString());
        }

        internal void Extends(string superClass)
        {
            _compiler.Unique(Q.Extends, ParseClassName(superClass));
        }

        internal void Vertex(string className)
        {
            _compiler.Unique(Q.Vertex, ParseClassName(className));
        }

        internal void Edge(string className)
        {
            _compiler.Unique(Q.Edge, ParseClassName(className));
        }

        internal void Insert<T>(T obj)
        {
            ODocument document;

            if (obj is ODocument)
            {
                document = obj as ODocument;
            }
            else
            {
                document = ODocument.ToDocument(obj);
            }

            if (!string.IsNullOrEmpty(document.OClassName))
            {
                Class(document.OClassName);
            }

            Set(document);
        }

        internal void Update<T>(T obj)
        {
            ODocument document;

            if (obj is ODocument)
            {
                document = obj as ODocument;
            }
            else
            {
                document = ODocument.ToDocument(obj);
            }

            if (!string.IsNullOrEmpty(document.OClassName))
            {
                Class(document.OClassName);
            }

            if (document.ORID != null)
            {
                Record(document.ORID);
            }

            Set(document);
        }

        internal void Delete<T>(T obj)
        {
            ODocument document;

            if (obj is ODocument)
            {
                document = obj as ODocument;
            }
            else
            {
                document = ODocument.ToDocument(obj);
            }

            if (!string.IsNullOrEmpty(document.OClassName))
            {
                Class(document.OClassName);
            }
            else
            {
                throw new OException(OExceptionType.Query, "Document doesn't contain OClassName value.");
            }

            if (document.ORID != null)
            {
                // check if the @rid is correct in real example
                Where("@rid");

                Equals(document.ORID);
            }
        }

        internal void DeleteVertex<T>(T obj)
        {
            ODocument document;

            if (obj is ODocument)
            {
                document = obj as ODocument;
            }
            else
            {
                document = ODocument.ToDocument(obj);
            }

            if (!string.IsNullOrEmpty(document.OClassName))
            {
                Class(document.OClassName);
            }

            if (document.ORID != null)
            {
                Record(document.ORID);
            }
        }

        internal void DeleteEdge<T>(T obj)
        {
            ODocument document;

            if (obj is ODocument)
            {
                document = obj as ODocument;
            }
            else
            {
                document = ODocument.ToDocument(obj);
            }

            if (!string.IsNullOrEmpty(document.OClassName))
            {
                Class(document.OClassName);
            }

            if (document.ORID != null)
            {
                Record(document.ORID);
            }
        }

        #region Select

        internal void Select(params string[] projections)
        {
            for (int i = 0; i < projections.Length; i++)
            {
                _compiler.Append(Q.Select, projections[i]);

                if (i < (projections.Length - 1))
                {
                    _compiler.Append(Q.Select, Q.Comma, "");
                }
            }
        }

        internal void Also(string projection)
        {
            _compiler.Append(Q.Select, Q.Comma, projection);
        }

        internal void Nth(int index)
        {
            _compiler.Append(Q.Select, "[" + index + "]");
        }

        internal void As(string alias)
        {
            _compiler.Append(Q.Select, "", Q.As, alias);
        }

        #endregion

        #region From

        internal void From(ORID orid)
        {
            _compiler.Unique(Q.From, orid.ToString());
        }

        internal void From(string target)
        {
            _compiler.Unique(Q.From, ParseClassName(target));
        }

        internal void From(OSqlSelect nestedSelect)
        {
            _compiler.Unique(Q.From, "(", nestedSelect.ToString(), ")");
        }

        internal void From(ODocument document)
        {
            if (!string.IsNullOrEmpty(document.OClassName))
            {
                From(document.OClassName);
            }

            if (document.ORID != null)
            {
                From(document.ORID);
            }
        }

        #endregion

        internal void To(ORID orid)
        {
            _compiler.Unique(Q.To, orid.ToString());
        }

        #region Set

        internal void Set<T>(string fieldName, T fieldValue)
        {
            string field = BuildFieldValue<T>(fieldName, fieldValue);

            _compiler.Append(Q.Set, field);
        }

        private string BuildFieldValue<T>(string fieldName, T fieldValue)
        {
            string field = "";

            if (_compiler.HasKey(Q.Set) && !string.IsNullOrWhiteSpace(fieldName))
            {
                field += ", ";
            }

            if (!string.IsNullOrWhiteSpace(fieldName))
                field += string.Join(" ", fieldName, Q.Equals, "");

            if (fieldValue == null)
            {
                field += "null";
            }
            else if (fieldValue is IList)
            {
                field += "[";
                IList collection = (IList)fieldValue;
                int iteration = 0;

                foreach (object item in collection)
                {
                    field += BuildFieldValue(null, item);

                    iteration++;

                    if (iteration < collection.Count)
                    {
                        field += ", ";
                    }
                }

                field += "]";
            }
            else if (fieldValue is IDictionary)
            {
                field += "{";
                int iteration = 0;
                IDictionary dict = (IDictionary)fieldValue;
                var enumerator = dict.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    iteration++;
                    if (enumerator.Value != null)
                    {
                        field += String.Format("'{0}':{1}", enumerator.Key, BuildFieldValue(null, enumerator.Value));

                        if (iteration < dict.Count)
                            field += ", ";
                    }

                }
                field += "}";
            }
            else
            {
                field += ToString(fieldValue);//.ToInvarianCultureString();
            }
            return field;
        }

        internal void Set<T>(T obj)
        {
            ODocument document;

            if (obj is ODocument)
            {
                document = obj as ODocument;
            }
            else
            {
                document = ODocument.ToDocument(obj);
            }

            // TODO: go also through embedded fields
            foreach (KeyValuePair<string, object> field in document)
            {
                // set only fields which doesn't start with @ character
                if ((field.Key.Length > 0) && (field.Key[0] != '@'))
                {
                    Set(field.Key, field.Value);
                }
            }
        }

        #endregion

        #region Where with conditions

        internal void Where(string field)
        {
            _compiler.Append(Q.Where, field);
        }

        internal void Where(IEnumerable<string> fields)
        {
            if (fields.Count() == 1)
            {
                Where(fields.First());
            }
            else
            {
                _compiler.Append(Q.Where, string.Concat("[", string.Join(",", fields), "]"));
            }
        }

        internal void And(string field)
        {
            _compiler.Append(Q.Where, "", Q.And, field);
        }

        internal void Or(string field)
        {
            _compiler.Append(Q.Where, "", Q.Or, field);
        }

        internal void Equals<T>(T item)
        {
            _compiler.Append(Q.Where, "", Q.Equals, ToString(item));
        }

        internal void NotEquals<T>(T item)
        {
            _compiler.Append(Q.Where, "", Q.NotEquals, ToString(item));
        }

        internal void Lesser<T>(T item)
        {
            _compiler.Append(Q.Where, "", Q.Lesser, ToString(item));
        }

        internal void LesserEqual<T>(T item)
        {
            _compiler.Append(Q.Where, "", Q.LesserEqual, ToString(item));
        }

        internal void Greater<T>(T item)
        {
            _compiler.Append(Q.Where, "", Q.Greater, ToString(item));
        }

        internal void GreaterEqual<T>(T item)
        {
            _compiler.Append(Q.Where, "", Q.GreaterEqual, ToString(item));
        }

        internal void Like<T>(T item)
        {
            _compiler.Append(Q.Where, "", Q.Like, ToString(item));
        }

        internal void Lucene<T>(T item)
        {
            _compiler.Append(Q.Where, "", Q.Lucene, ToString(item));
        }

        internal void IsNull()
        {
            _compiler.Append(Q.Where, "", Q.Is, Q.Null);
        }

        internal void Contains<T>(T item)
        {
            _compiler.Append(Q.Where, "", Q.Contains, ToString(item));
        }

        internal void Contains<T>(string field, T value)
        {
            _compiler.Append(Q.Where, "", Q.Contains, "(" + field, Q.Equals, ToString(value) + ")");
        }

        internal void In<T>(IList<T> list)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[");
            for (var i = 0; i < list.Count; i++)
            {
                builder.Append(ToString(list[i]));
                if (i != list.Count - 1)
                {
                    builder.Append(",");
                }
            }
            builder.Append("]");
            _compiler.Append(Q.Where, "", Q.In, ToString(builder));
        }

        internal void Between(int num1, int num2)
        {
            _compiler.Append(Q.Where, "", Q.Between, ToString(num1), Q.And, ToString(num2));
        }
        #endregion

        #region Add

        internal void Add<T>(string fieldName, T fieldValue)
        {
            string field = "";

            if (_compiler.HasKey(Q.Add))
            {
                field += ", ";
            }

            field += string.Join(" ", fieldName, Q.Equals, ToString(fieldValue));

            _compiler.Append(Q.Add, field);
        }

        #endregion

        #region Remove

        public void Remove(string fieldName)
        {
            if (_compiler.HasKey(Q.Remove))
            {
                fieldName = ", " + fieldName;
            }

            _compiler.Append(Q.Remove, fieldName);
        }

        public void Remove<T>(string fieldName, T collectionValue)
        {
            if (_compiler.HasKey(Q.Remove))
            {
                fieldName = ", " + fieldName;
            }

            _compiler.Append(Q.Remove, fieldName, Q.Equals, ToString(collectionValue));
        }

        #endregion

        #region Upsert

        public void Upsert()
        {
            _compiler.Unique(Q.Upsert);
        }

        #endregion

        internal void OrderBy(params string[] fields)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                _compiler.Append(Q.OrderBy, fields[i]);

                if (i < (fields.Length - 1))
                {
                    _compiler.Append(Q.OrderBy, Q.Comma, "");
                }
            }
        }

        internal void Ascending()
        {
            _compiler.Unique(Q.Ascending, Q.Ascending);
        }

        internal void Descending()
        {
            _compiler.Unique(Q.Descending, Q.Descending);
        }

        internal void Skip(int skipCount)
        {
            _compiler.Unique(Q.Skip, skipCount.ToString());
        }

        internal void Limit(int maxRecords)
        {
            _compiler.Unique(Q.Limit, maxRecords.ToString());
        }

        #region ToString

        private string ToString(object value)
        {
            string sql = "";

            if (value is string)
            {
                sql = string.Join(" ", "'" + ((string)value).Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("'", "\\'") + "'");
            }
            else if (value is DateTime)
            {
                if (_connection == null)
                {
                    //DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    DateTime fieldValue = (DateTime)((object)value);
                    //field += ((long)(value - unixEpoch).TotalMilliseconds);
                    sql = "'" + fieldValue.ToString("s").Replace('T', ' ') + "'";
                }
                else
                {
                    var propDocument = _connection.Database.DatabaseProperties;
                    var dateTimeFormat = propDocument.GetField<string>("DateTimeFormat");
                    var timeZone = propDocument.GetField<string>("Timezone");
                    dateTimeFormat = Regex.Replace(dateTimeFormat, @"\.SSS", ".fff");
                    dateTimeFormat = Regex.Replace(dateTimeFormat, @"\.SS", ".ff");

                    // How to map Windows TimeZone id to IANA timezone id

                    DateTime fieldValue = (DateTime)((object)value);
                    sql = "'" + fieldValue.ToString(dateTimeFormat) + "'";
                }
            }
            else if (value is TimeSpan)
            {
                sql = "'" + value.ToString() + "'";
            }
            else if (value is ODocument)
            {
                var document = ((ODocument)value);

                var properties = document.Where(item => item.Key[0] != '@');
                var propertiesLength = properties.Count();

                sql += "{";

                int iteration = 0;
                foreach (KeyValuePair<string, object> field in properties)
                {
                    if (field.Key.Length > 0)
                    {
                        var strValue = BuildFieldValue("'" + field.Key + "'", field.Value).Replace('=', ':');
                        if (strValue[0] == ',')
                            sql += strValue.Substring(1);
                        else
                            sql += strValue;

                        iteration++;

                        if (iteration < properties.Count())
                            sql += ",";
                    }
                }

                if (!string.IsNullOrEmpty(document.OClassName))
                    sql += ",'@class':'" + document.OClassName + "'";//,";

                //sql += "'@type':'d','@version':" + document.OVersion;
                sql += "}";
            }
            else if (value is Guid || value is Enum)
            {
                sql = string.Join(" ", "'" + value.ToInvarianCultureString() + "'");
            }
            else if (value is Decimal)
            {
                sql = string.Join(" ", value.ToInvarianCultureString() + "d");
                // Experimental function https://github.com/orientechnologies/orientdb/issues/3483
                // sql = string.Join(" ", "decimal(", value.ToInvarianCultureString(), ")");
                // Bug in orientdb #3483 after that use suffix + "c");
            }
            else if (value is float)
            {
                sql = string.Join(" ", value.ToInvarianCultureString() + "f");
            }
            else if (value is double)
            {
                sql = string.Join(" ", value.ToInvarianCultureString() + "d");
            }
            else if (value == null)
            {
                sql = string.Join(" ", "null");
            }
            else
            {
                sql = string.Join(" ", value.ToInvarianCultureString());
            }

            return sql;
        }

        internal string ToString(QueryType type)
        {
            switch (type)
            {
                case QueryType.CreateClass:
                    return GenerateCreateClassQuery();
                case QueryType.CreateProperty:
                    return GenerateCreatePropertyQuery();
                case QueryType.CreateCluster:
                    return GenerateCreateClusterQuery();
                case QueryType.CreateEdge:
                    return GenerateCreateEdgeQuery();
                case QueryType.CreateVertex:
                    return GenerateCreateVertexQuery();
                case QueryType.DeleteVertex:
                    return GenerateDeleteVertexQuery();
                case QueryType.DeleteEdge:
                    return GenerateDeleteEdgeQuery();
                case QueryType.DeleteDocument:
                    return GenerateDeleteDocumentQuery();
                case QueryType.Insert:
                    return GenerateInsertQuery();
                case QueryType.Update:
                    return GenerateUpdateQuery();
                case QueryType.Select:
                    return GenerateSelectQuery();
                default:
                    break;
            }

            return "";
        }

        #endregion

        private string GenerateCreateClassQuery()
        {
            string query = "";

            // CREATE CLASS <class> 
            query += string.Join(" ", Q.Create, Q.Class, _compiler.Value(Q.Class));

            // [EXTENDS <super-class>] 
            if (_compiler.HasKey(Q.Extends))
            {
                query += string.Join(" ", "", Q.Extends, _compiler.Value(Q.Extends));
            }

            // [CLUSTER <clusterId>*]
            if (_compiler.HasKey(Q.Cluster))
            {
                query += string.Join(" ", "", Q.Cluster, _compiler.Value(Q.Cluster));
            }

            return query;
        }

        private string GenerateCreatePropertyQuery()
        {
            // CREATE PROPERTY <class>.<property> <type> [<linked-type>|<linked-class>]
            string query = "";

            // CREATE PROPERTY <class> 
            query += string.Join(" ", Q.Create, Q.Property, ParsePropertyName(_compiler.Value(Q.Property)));
            // [<linked-type>|<linked-class>]
            query += string.Join(" ", "", _compiler.OrderedValue(Q.LinkedType, Q.LinkedClass));

            return query;
        }

        private string GenerateCreateClusterQuery()
        {
            string query = "";

            // CREATE CLUSTER <name> <type> 
            query += string.Join(" ", Q.Create, Q.Cluster, _compiler.Value(Q.Cluster));

            // [DATASEGMENT <data-segment>|default] 
            // TODO:

            // [LOCATION <path>|default] 
            // TODO:

            // [POSITION <position>|append]
            // TODO:

            return query;
        }

        private string GenerateCreateEdgeQuery()
        {
            string query = "";

            // CREATE EDGE [<class>] 
            query += string.Join(" ", Q.Create, Q.Edge, _compiler.Value(Q.Edge));

            // [CLUSTER <cluster>]
            if (_compiler.HasKey(Q.Cluster))
            {
                query += string.Join(" ", "", Q.Cluster, _compiler.Value(Q.Cluster));
            }

            // FROM <rid>|(<query>)|[<rid>]* 
            query += string.Join(" ", "", Q.From, _compiler.Value(Q.From));

            // TO <rid>|(<query>)|[<rid>]* 
            query += string.Join(" ", "", Q.To, _compiler.Value(Q.To));

            // [SET <field> = <expression>[,]*]
            if (_compiler.HasKey(Q.Set))
            {
                query += string.Join(" ", "", Q.Set, _compiler.Value(Q.Set));
            }

            return query;
        }

        private string GenerateCreateVertexQuery()
        {
            string query = "";

            // CREATE VERTEX [<class>] 
            query += string.Join(" ", Q.Create, Q.Vertex, _compiler.Value(Q.Vertex));

            // [CLUSTER <cluster>]
            if (_compiler.HasKey(Q.Cluster))
            {
                query += string.Join(" ", "", Q.Cluster, _compiler.Value(Q.Cluster));
            }

            // [SET <field> = <expression>[,]*]
            if (_compiler.HasKey(Q.Set))
            {
                query += string.Join(" ", "", Q.Set, _compiler.Value(Q.Set));
            }

            return query;
        }

        private string GenerateInsertQuery()
        {
            string query = "";

            // INSERT INTO <Class>|cluster:<cluster>|index:<index> 
            query += string.Join(" ", Q.Insert, Q.Into, _compiler.Value(Q.Class));

            // [<cluster>](cluster) 
            if (_compiler.HasKey(Q.Cluster))
            {
                query += string.Join(" ", "", Q.Cluster, _compiler.Value(Q.Cluster));
            }

            // [VALUES (<expression>[,]((<field>[,]*))*)]|[<field> = <expression>[,](SET)*]
            if (_compiler.HasKey(Q.Set))
            {
                query += string.Join(" ", "", Q.Set, _compiler.Value(Q.Set));
            }

            return query;
        }

        private string GenerateUpdateQuery()
        {
            string query = "";

            // UPDATE <class>|cluster:<cluster>|<recordID>
            query += string.Join(" ", Q.Update, _compiler.OrderedValue(Q.Class, Q.Cluster, Q.Record));

            // [SET|INCREMENT <field-name> = <field-value>](,)*
            if (_compiler.HasKey(Q.Set))
            {
                query += string.Join(" ", "", Q.Set, _compiler.Value(Q.Set));
            }

            // (ADD|REMOVE])[<field-name> = <field-value>](,)*
            if (_compiler.HasKey(Q.Add))
            {
                query += string.Join(" ", "", Q.Add, _compiler.Value(Q.Add));
            }
            else if (_compiler.HasKey(Q.Remove))
            {
                query += string.Join(" ", "", Q.Remove, _compiler.Value(Q.Remove));
            }

            // (UPSERT)
            if (_compiler.HasKey(Q.Upsert))
            {
                query += " " + Q.Upsert;
            }

            // [<conditions>](WHERE) 
            if (_compiler.HasKey(Q.Where))
            {
                query += string.Join(" ", "", Q.Where, _compiler.Value(Q.Where));
            }

            // [<max-records>](LIMIT)
            if (_compiler.HasKey(Q.Limit))
            {
                query += string.Join(" ", "", Q.Limit, _compiler.Value(Q.Limit));
            }

            return query;
        }

        private string GenerateDeleteVertexQuery()
        {
            string query = "";

            // DELETE VERTEX <rid>|<[<class>]
            query += string.Join(" ", Q.Delete, Q.Vertex, _compiler.OrderedValue(Q.Class, Q.Record));

            // [WHERE <conditions>] 
            if (_compiler.HasKey(Q.Where))
            {
                query += string.Join(" ", "", Q.Where, _compiler.Value(Q.Where));
            }

            // [LIMIT <MaxRecords>>]
            if (_compiler.HasKey(Q.Limit))
            {
                query += string.Join(" ", "", Q.Limit, _compiler.Value(Q.Limit));
            }

            return query;
        }

        private string GenerateDeleteEdgeQuery()
        {
            string query = "";

            // DELETE EDGE <rid>|FROM <rid>|TO <rid>|<[<class>] 
            if (_compiler.HasKey(Q.From) && _compiler.HasKey(Q.To))
            {
                query += string.Join(" ", Q.Delete, Q.Edge, Q.From, _compiler.Value(Q.From), Q.To, _compiler.Value(Q.To));
            }
            else
            {
                query += string.Join(" ", Q.Delete, Q.Edge, _compiler.OrderedValue(Q.Class, Q.Record));
            }

            // [WHERE <conditions>]> 
            if (_compiler.HasKey(Q.Where))
            {
                query += string.Join(" ", "", Q.Where, _compiler.Value(Q.Where));
            }

            // [LIMIT <MaxRecords>]
            if (_compiler.HasKey(Q.Limit))
            {
                query += string.Join(" ", "", Q.Limit, _compiler.Value(Q.Limit));
            }

            return query;
        }

        private string GenerateDeleteDocumentQuery()
        {
            string query = "";

            // DELETE FROM <Class>|cluster:<cluster>|index:<index> 
            query += string.Join(" ", Q.Delete, Q.From, _compiler.OrderedValue(Q.Class, Q.Cluster));

            // [<Condition>*](WHERE) 
            if (_compiler.HasKey(Q.Where))
            {
                query += string.Join(" ", "", Q.Where, _compiler.Value(Q.Where));
            }

            // [BY <Fields>* [ASC|DESC](ORDER)*] 
            // TODO:


            // [<MaxRecords>](LIMIT)
            if (_compiler.HasKey(Q.Limit))
            {
                query += string.Join(" ", "", Q.Limit, _compiler.Value(Q.Limit));
            }

            return query;
        }

        private string GenerateSelectQuery()
        {
            string query = "";

            // SELECT [<Projections>]
            if (string.IsNullOrEmpty(_compiler.Value(Q.Select)))
            {
                query += string.Join(" ", Q.Select);
            }
            else
            {
                query += string.Join(" ", Q.Select, _compiler.Value(Q.Select));
            }

            // [LET <Assignment>*]) 
            // TODO:

            // FROM <Target>
            query += string.Join(" ", "", Q.From, _compiler.Value(Q.From));

            // [<Condition>*](WHERE) 
            if (_compiler.HasKey(Q.Where))
            {
                query += string.Join(" ", "", Q.Where, _compiler.Value(Q.Where));
            }

            // [BY <Field>](GROUP) 
            // TODO:

            // [BY <Fields>* [ASC|DESC](ORDER)*] 
            if (_compiler.HasKey(Q.OrderBy))
            {
                query += string.Join(" ", "", Q.OrderBy, _compiler.Value(Q.OrderBy));
            }

            if (_compiler.HasKey(Q.Ascending))
            {
                query += string.Join(" ", "", Q.Ascending);
            }

            if (_compiler.HasKey(Q.Descending))
            {
                query += string.Join(" ", "", Q.Descending);
            }

            // [<SkipRecords>](SKIP) 
            if (_compiler.HasKey(Q.Skip))
            {
                query += string.Join(" ", "", Q.Skip, _compiler.Value(Q.Skip));
            }

            // [<MaxRecords>](LIMIT)
            if (_compiler.HasKey(Q.Limit))
            {
                query += string.Join(" ", "", Q.Limit, _compiler.Value(Q.Limit));
            }

            return query;
        }

        private string ParseClassName(string className)
        {
            if (className.Equals(typeof(OVertex).Name))
            {
                return "V";
            }

            if (className.Equals(typeof(OEdge).Name))
            {
                return "E";
            }

            return className;
        }

        private string ParsePropertyName(string propertyName)
        {
            if (_compiler.HasKey(Q.Class))
            {
                return _compiler[Q.Class] + "." + propertyName;
            }

            return propertyName;
        }

    }
}
