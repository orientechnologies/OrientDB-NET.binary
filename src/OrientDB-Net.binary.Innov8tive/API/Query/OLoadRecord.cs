using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

namespace Orient.Client.API.Query
{
    public class OLoadRecord
    {
        private readonly Connection _connection;
        private ORID _orid;
        private string _fetchPlan = string.Empty;

        internal OLoadRecord(Connection connection)
        {
            _connection = connection;
        }

        public OLoadRecord ORID(ORID orid)
        {
            _orid = orid;
            return this;
        }

        public OLoadRecord FetchPlan(string plan)
        {
            _fetchPlan = plan;
            return this;
        }

        public ODocument Run()
        {
            var operation = new LoadRecord(_orid, _fetchPlan, _connection.Database);
            var result = new OCommandResult(_connection.ExecuteOperation(operation));
            return result.ToSingle().To<ODocument>();
        }
        public T Run<T>()
            where T : class, new()
        {
            var operation = new LoadRecord(_orid, _fetchPlan, _connection.Database);
            var result = new OCommandResult(_connection.ExecuteOperation(operation));
            return result.ToSingle().To<T>();
        }
    }
}
