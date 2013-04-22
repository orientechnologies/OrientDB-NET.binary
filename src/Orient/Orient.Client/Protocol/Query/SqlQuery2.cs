using System.Collections;
using System.Collections.Generic;

namespace Orient.Client.Protocol
{
    internal class SqlQuery2
    {
        private QueryCompiler _compiler = new QueryCompiler();

        internal void Class(string className)
        {
            _compiler.Unique(Q.Class, className);
        }

        #region Cluster

        internal void Cluster(string clusterName)
        {
            _compiler.Unique(Q.Cluster, clusterName);
        }

        internal void Cluster(string clusterName, OClusterType clustertype)
        {
            _compiler.Unique(Q.Cluster, clusterName, clustertype.ToString().ToUpper());
        }

        #endregion

        internal void Record(ORID orid)
        {
            _compiler.Unique(Q.Record, orid.ToString());
        }

        internal void Extends(string superClass)
        {
            _compiler.Unique(Q.Extends, superClass);
        }

        internal void Vertex(string className)
        {
            _compiler.Unique(Q.Vertex, className);
        }

        internal void Edge(string className)
        {
            _compiler.Unique(Q.Edge, className);
        }

        internal void Insert(ODocument document)
        {
            if (!string.IsNullOrEmpty(document.OClassName))
            {
                Class(document.OClassName);
            }

            Set(document);
        }

        #region Update

        internal void Update(ORID orid)
        {
            Record(orid);
        }

        internal void Update(ODocument document)
        {
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

        #endregion

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
            _compiler.Unique(Q.From, target);
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
            string field = "";

            if (_compiler.HasKey(Q.Set))
            {
                field += ", ";
            }

            field += string.Join(" ", fieldName, Q.Equals, "");

            if (fieldValue == null)
            {
                field += "null";
            }
            else if (fieldValue is string)
            {
                field += "'" + fieldValue + "'";
            }
            else if (fieldValue is IList)
            {
                field += "[";
                IList collection = (IList)fieldValue;
                int iteration = 0;

                foreach (object item in collection)
                {
                    field += SqlQuery.ToString(item);

                    iteration++;

                    if (iteration < collection.Count)
                    {
                        field += ", ";
                    }
                }

                field += "]";
            }
            else
            {
                field += fieldValue.ToString();
            }

            _compiler.Append(Q.Set, field);
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
                document = ODocument.ToDocument<T>(obj);
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
            _compiler.Append(Q.Where, "", Q.Equals, SqlQuery2.ToString(item));
        }

        internal void NotEquals<T>(T item)
        {
            _compiler.Append(Q.Where, "", Q.NotEquals, SqlQuery2.ToString(item));
        }

        internal void Lesser<T>(T item)
        {
            _compiler.Append(Q.Where, "", Q.Lesser, SqlQuery2.ToString(item));
        }

        internal void LesserEqual<T>(T item)
        {
            _compiler.Append(Q.Where, "", Q.LesserEqual, SqlQuery2.ToString(item));
        }

        internal void Greater<T>(T item)
        {
            _compiler.Append(Q.Where, "", Q.Greater, SqlQuery2.ToString(item));
        }

        internal void GreaterEqual<T>(T item)
        {
            _compiler.Append(Q.Where, "", Q.GreaterEqual, SqlQuery2.ToString(item));
        }

        internal void Like<T>(T item)
        {
            _compiler.Append(Q.Where, "", Q.Like, SqlQuery2.ToString(item));
        }

        internal void IsNull()
        {
            _compiler.Append(Q.Where, "", Q.Is, Q.Null);
        }

        internal void Contains<T>(T item)
        {
            _compiler.Append(Q.Where, "", Q.Contains, SqlQuery2.ToString(item));
        }

        internal void Contains<T>(string field, T value)
        {
            _compiler.Append(Q.Where, "", Q.Contains, "(" + field, Q.Equals, SqlQuery2.ToString(value) + ")");
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

            field += string.Join(" ", fieldName, Q.Equals, SqlQuery.ToString(fieldValue));

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

            _compiler.Append(Q.Remove, fieldName, Q.Equals, SqlQuery2.ToString(collectionValue));
        }

        #endregion

        #region ToString

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

        internal string ToString(QueryType type)
        {
            switch (type)
            {
                case QueryType.CreateClass:
                    return GenerateCreateClassQuery();
                case QueryType.CreateCluster:
                    return GenerateCreateClusterQuery();
                case QueryType.CreateEdge:
                    return GenerateCreateEdgeQuery();
                case QueryType.CreateVertex:
                    return GenerateCreateVertexQuery();
                case QueryType.Delete:
                    break;
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

            // [<conditions>](WHERE) 
            if (_compiler.HasKey(Q.Where))
            {
                query += string.Join(" ", "", Q.Where, _compiler.Value(Q.Where));
            }

            // [<max-records>](LIMIT)
            // TODO:

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
            // TODO:

            // [<SkipRecords>](SKIP) 
            // TODO:

            // [<MaxRecords>](LIMIT)
            // TODO:

            return query;
        }
    }
}
