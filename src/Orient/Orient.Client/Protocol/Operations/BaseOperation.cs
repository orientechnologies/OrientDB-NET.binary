using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    abstract class BaseOperation : IOperation
    {
        protected ODatabase _database;

        public BaseOperation(ODatabase database)
        {
            _database = database;
        }

        public IRecordSerializer Serializer
        {
            get { return RecordSerializerFactory.GetSerializer(_database); }
        }

        public abstract ODocument Response(Response response);

        public abstract Request Request(Request request);

    }
}
