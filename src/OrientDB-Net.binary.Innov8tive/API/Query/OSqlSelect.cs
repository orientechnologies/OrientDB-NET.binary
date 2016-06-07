using System.Collections.Generic;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;
using Orient.Client.Protocol.Operations.Command;
using Orient.Client.API.Attributes;
using System;
using System.Reflection;

// syntax:
// SELECT [FROM <Target> 
// [LET <Assignment>*](<Projections>]) 
// [<Condition>*](WHERE) 
// [BY <Field>](GROUP) 
// [BY <Fields>* [ASC|DESC](ORDER)*] 
// [<SkipRecords>](SKIP) 
// [<MaxRecords>](LIMIT)

namespace Orient.Client
{
    public class OSqlSelect
    {
        private SqlQuery _sqlQuery;
        private Connection _connection;

        public OSqlSelect()
        {
            _sqlQuery = new SqlQuery(null);
        }
        internal OSqlSelect(Connection connection)
        {
            _connection = connection;
            _sqlQuery = new SqlQuery(connection);
        }

        #region Select

        public OSqlSelect Select(params string[] projections)
        {
            _sqlQuery.Select(projections);

            return this;
        }

        public OSqlSelect Also(string projection)
        {
            _sqlQuery.Also(projection);

            return this;
        }

        /*public OSqlSelect First()
        {
            _sqlQuery.Surround("first");

            return this;
        }*/

        public OSqlSelect Nth(int index)
        {
            _sqlQuery.Nth(index);

            return this;
        }

        public OSqlSelect As(string alias)
        {
            _sqlQuery.As(alias);

            return this;
        }

        #endregion

        #region From

        public OSqlSelect From(string target)
        {
            _sqlQuery.From(target);

            return this;
        }

        public OSqlSelect From(ORID orid)
        {
            _sqlQuery.From(orid);

            return this;
        }

        public OSqlSelect From(OSqlSelect nestedSelect)
        {
            _sqlQuery.From(nestedSelect);
            return this;
        }

        public OSqlSelect From(ODocument document)
        {
            if ((document.ORID == null) && string.IsNullOrEmpty(document.OClassName))
            {
                throw new OException(OExceptionType.Query, "Document doesn't contain ORID or OClassName value.");
            }

            _sqlQuery.From(document);

            return this;
        }

        public OSqlSelect From<T>()
        {
            //GRD: I think this would be the same thing?
            MemberInfo[] membersInfo = typeof(T).GetTypeInfo().GetProperties();
            foreach (MemberInfo memberInfo in membersInfo)
            {
                foreach (object attribute in memberInfo.GetCustomAttributes(typeof(OAliasAttribute), true))
                {
                    return From(((OAliasAttribute)attribute).Name);
                }
            }

            return From(typeof(T).Name);
            

            //var tAttribute = (OAliasAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(OAliasAttribute), true);
            //if (tAttribute == null)
            //{
            //    return From(typeof(T).Name);
            //}
            //else
            //{
            //    return From(tAttribute.Name);
            //}
        }

        #endregion

        #region Where with conditions

        public OSqlSelect Where(params string[] fields)
        {
            _sqlQuery.Where(fields);

            return this;
        }

        public OSqlSelect Where(IEnumerable<string> fields)
        {
            _sqlQuery.Where(fields);

            return this;
        }

        public OSqlSelect And(string field)
        {
            _sqlQuery.And(field);

            return this;
        }

        public OSqlSelect Or(string field)
        {
            _sqlQuery.Or(field);

            return this;
        }

        public OSqlSelect Equals<T>(T item)
        {
            _sqlQuery.Equals<T>(item);

            return this;
        }

        public OSqlSelect NotEquals<T>(T item)
        {
            _sqlQuery.NotEquals<T>(item);

            return this;
        }

        public OSqlSelect Lesser<T>(T item)
        {
            _sqlQuery.Lesser<T>(item);

            return this;
        }

        public OSqlSelect LesserEqual<T>(T item)
        {
            _sqlQuery.LesserEqual<T>(item);

            return this;
        }

        public OSqlSelect Greater<T>(T item)
        {
            _sqlQuery.Greater<T>(item);

            return this;
        }

        public OSqlSelect GreaterEqual<T>(T item)
        {
            _sqlQuery.GreaterEqual<T>(item);

            return this;
        }

        public OSqlSelect Like<T>(T item)
        {
            _sqlQuery.Like<T>(item);

            return this;
        }

        public OSqlSelect Lucene<T>(T item)
        {
            _sqlQuery.Lucene<T>(item);

            return this;
        }


        public OSqlSelect IsNull()
        {
            _sqlQuery.IsNull();

            return this;
        }

        public OSqlSelect Contains<T>(T item)
        {
            _sqlQuery.Contains<T>(item);

            return this;
        }

        public OSqlSelect Contains<T>(string field, T value)
        {
            _sqlQuery.Contains<T>(field, value);

            return this;
        }

        public OSqlSelect In<T>(IList<T> list)
        {
            _sqlQuery.In(list);

            return this;
        }

        public OSqlSelect Between(int num1, int num2)
        {
            _sqlQuery.Between(num1, num2);
            return this;
        }
        #endregion

        public OSqlSelect OrderBy(params string[] fields)
        {
            _sqlQuery.OrderBy(fields);

            return this;
        }

        public OSqlSelect Ascending()
        {
            _sqlQuery.Ascending();

            return this;
        }

        public OSqlSelect Descending()
        {
            _sqlQuery.Descending();

            return this;
        }

        public OSqlSelect Skip(int skipCount)
        {
            _sqlQuery.Skip(skipCount);

            return this;
        }

        public OSqlSelect Limit(int maxRecords)
        {
            _sqlQuery.Limit(maxRecords);

            return this;
        }

        #region ToList

        public List<T> ToList<T>() where T : class, new()
        {
            List<T> result = new List<T>();
            List<ODocument> documents = ToList("*:0");

            foreach (ODocument document in documents)
            {
                result.Add(document.To<T>());
            }

            return result;
        }

        public List<ODocument> ToList()
        {
            return ToList("*:0");
        }

        public List<ODocument> ToList(string fetchPlan)
        {
            CommandPayloadQuery payload = new CommandPayloadQuery();
            payload.Text = ToString();
            payload.NonTextLimit = -1;
            payload.FetchPlan = fetchPlan;
            //payload.SerializedParams = new byte[] { 0 };

            Command operation = new Command(_connection.Database);
            operation.OperationMode = OperationMode.Asynchronous;
            operation.CommandPayload = payload;

            OCommandResult commandResult = new OCommandResult(_connection.ExecuteOperation(operation));

            return commandResult.ToList();
        }

        #endregion

        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.Select);
        }
    }
}
