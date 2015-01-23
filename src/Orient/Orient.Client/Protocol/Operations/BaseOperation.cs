using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        protected bool EndOfStream(BinaryReader reader)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var length = (Int32)typeof(BufferedStream).GetField("_readLen", flags).GetValue(reader.BaseStream);
            var pos = (Int32)typeof(BufferedStream).GetField("_readPos", flags).GetValue(reader.BaseStream);
            return length == pos;
        }
    }
}
