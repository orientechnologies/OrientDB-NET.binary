using System.Collections.Generic;
using Orient.Client.Protocol;

namespace Orient.Client
{
    public class OSqlCreate
    {
        // syntax: CREATE CLASS <class> [EXTENDS <super-class>] [CLUSTER <clusterId>*]
        #region Class

        public string Class(string className)
        {
            return Class(className, null, null);
        }

        public string Class(string className, string extends)
        {
            return Class(className, extends, null);
        }

        public string Class(string className, string extends, string cluster)
        {
            string sql = string.Join(" ", Q.Create, Q.Class, className);

            if (!string.IsNullOrEmpty(extends))
            {
                sql += string.Join(" ", "", Q.Extends, extends);
            }

            if (!string.IsNullOrEmpty(cluster))
            {
                sql += string.Join(" ", "", Q.Cluster, cluster);
            }

            return sql;
        }

        #endregion

        // syntax CREATE CLUSTER <name> <type> [DATASEGMENT <data-segment>|default] [LOCATION <path>|default] [POSITION <position>|append]
        #region Cluster

        public string Cluster(string clusterName, OClusterType clusterType)
        {
            string sql = string.Join(" ", Q.Create, Q.Cluster, clusterName, clusterType.ToString().ToUpper());

            return sql;
        }

        #endregion

        // syntax: CREATE EDGE [<class>] [CLUSTER <cluster>] FROM <rid>|(<query>)|[<rid>]* TO <rid>|(<query>)|[<rid>]* [SET <field> = <expression>[,]*]
        #region Edge

        public string Edge(string className, ORID from, ORID to)
        {
            return Edge(className, null, from, to, null);
        }

        public string Edge(string className, ORID from, ORID to, ODataObject fields)
        {
            return Edge(className, null, from, to, fields);
        }

        public string Edge(string className, string cluster, ORID from, ORID to)
        {
            return Edge(className, cluster, from, to, null);
        }

        public string Edge(string className, string cluster, ORID from, ORID to, ODataObject fields)
        {
            string sql = string.Join(" ", Q.Create, Q.Edge, className);

            if (!string.IsNullOrEmpty(cluster))
            {
                sql += string.Join(" ", "", Q.Cluster, cluster);
            }

            sql += string.Join(" ", "", Q.From, from.ToString(), Q.To, to.ToString());

            if ((fields != null) && (fields.Count > 0))
            {
                sql += string.Join(" ", "", Q.Set);
                int iteration = 0;

                // TODO: go also through embedded fields
                foreach (KeyValuePair<string, object> field in fields)
                {
                    sql += string.Join(" ", "", field.Key, "=");

                    if (field.Value is string)
                    {
                        sql += string.Join(" ", "", "'" + field.Value + "'");
                    }
                    else
                    {
                        sql += string.Join(" ", "", field.Value.ToString());
                    }

                    iteration++;

                    if (iteration < fields.Count)
                    {
                        sql += ",";
                    }
                }
            }

            return sql;
        }

        #endregion

        // syntax: CREATE VERTEX [<class>] [CLUSTER <cluster>] [SET <field> = <expression>[,]*]
        #region Vertex

        public string Vertex(string className, ODataObject fields)
        {
            return Vertex(className, null, fields);
        }

        public string Vertex(string className, string cluster)
        {
            return Vertex(className, cluster, null);
        }

        public string Vertex(string className, string cluster, ODataObject fields)
        {
            string sql = string.Join(" ", Q.Create, Q.Vertex, className);

            if (!string.IsNullOrEmpty(cluster))
            {
                sql += string.Join(" ", "", Q.Cluster, cluster);
            }

            if ((fields != null) && (fields.Count > 0))
            {
                sql += string.Join(" ", "", Q.Set);
                int iteration = 0;

                // TODO: go also through embedded fields
                foreach (KeyValuePair<string, object> field in fields)
                {
                    sql += string.Join(" ", "", field.Key, "=");

                    if (field.Value is string)
                    {
                        sql += string.Join(" ", "", "'" + field.Value + "'");
                    }
                    else
                    {
                        sql += string.Join(" ", "", field.Value.ToString());
                    }

                    iteration++;

                    if (iteration < fields.Count)
                    {
                        sql += ",";
                    }
                }
            }

            return sql;
        }

        #endregion
    }
}
