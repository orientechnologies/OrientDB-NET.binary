using System.Collections.Generic;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

namespace Orient.Client.Sql
{
    public class OSqlCreate
    {
        private Connection _connection;

        internal OSqlCreate(Connection connection)
        {
            _connection = connection;
        }

        // syntax: CREATE CLASS <class> [EXTENDS <super-class>] [CLUSTER <clusterId>*]
        #region Class

        public short Class(string className)
        {
            return Class(className, null, null);
        }

        public short Class(string className, string extends)
        {
            return Class(className, extends, null);
        }

        public short Class(string className, string extends, short? clusterId)
        {
            string sql = string.Join(" ", Q.Create, Q.Class, className);

            if (!string.IsNullOrEmpty(extends))
            {
                sql += string.Join(" ", "", Q.Extends, extends);
            }

            if (clusterId.HasValue)
            {
                sql += string.Join(" ", "", Q.Cluster, clusterId.Value);
            }

            OCommandResult commandResult = Execute(sql);

            return short.Parse(commandResult.ToDataObject().Get<string>("Content"));
        }

        #endregion

        // syntax CREATE CLUSTER <name> <type> [DATASEGMENT <data-segment>|default] [LOCATION <path>|default] [POSITION <position>|append]
        #region Cluster

        public short Cluster(string clusterName, OClusterType clusterType)
        {
            string sql = string.Join(" ", Q.Create, Q.Cluster, clusterName, clusterType.ToString().ToUpper());

            OCommandResult commandResult = Execute(sql);

            return short.Parse(commandResult.ToDataObject().Get<string>("Content"));
        }

        #endregion

        // syntax: CREATE EDGE [<class>] [CLUSTER <cluster>] FROM <rid>|(<query>)|[<rid>]* TO <rid>|(<query>)|[<rid>]* [SET <field> = <expression>[,]*]
        #region Edge

        public ORecord Edge(string className, ORID from, ORID to)
        {
            return Edge(className, null, from, to, null);
        }

        public ORecord Edge(string className, ORID from, ORID to, ODataObject fields)
        {
            return Edge(className, null, from, to, fields);
        }

        public ORecord Edge(string className, string cluster, ORID from, ORID to)
        {
            return Edge(className, cluster, from, to, null);
        }

        public ORecord Edge(string className, string cluster, ORID from, ORID to, ODataObject fields)
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

            OCommandResult commandResult = Execute(sql);

            return commandResult.ToSingle();
        }

        #endregion

        // syntax: CREATE VERTEX [<class>] [CLUSTER <cluster>] [SET <field> = <expression>[,]*]
        #region Vertex

        public ORecord Vertex(string className, ODataObject fields)
        {
            return Vertex(className, null, fields);
        }

        public ORecord Vertex(string className, string cluster)
        {
            return Vertex(className, cluster, null);
        }

        public ORecord Vertex(string className, string cluster, ODataObject fields)
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

            OCommandResult commandResult = Execute(sql);

            return commandResult.ToSingle();
        }

        #endregion

        private OCommandResult Execute(string sql)
        {
            CommandPayload payload = new CommandPayload();
            payload.Type = CommandPayloadType.Sql;
            payload.Text = sql;
            payload.NonTextLimit = -1;
            payload.FetchPlan = "";
            payload.SerializedParams = new byte[] { 0 };

            Command operation = new Command();
            operation.OperationMode = OperationMode.Synchronous;
            operation.ClassType = CommandClassType.NonIdempotent;
            operation.CommandPayload = payload;

            return new OCommandResult(sql, _connection.ExecuteOperation<Command>(operation));
        }
    }
}
