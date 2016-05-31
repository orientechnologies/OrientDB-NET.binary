using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

namespace Orient.Client.API.Query
{
    public class ORecordMetadata
    {
        private readonly Connection _connection;
        private ORID _orid;

        internal ORecordMetadata(Connection connection)
        {
            _connection = connection;
        }

        public ORecordMetadata ORID(ORID orid)
        {
            _orid = orid;
            return this;
        }

        public ODocument Run()
        {
            var operation = new RecordMetadata(_orid, _connection.Database);
            return _connection.ExecuteOperation(operation);
        }
    }
}
