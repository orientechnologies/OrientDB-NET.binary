using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Orient.Client.Protocol.Serializers;
using System.Net.Sockets;

namespace Orient.Client.Protocol.Operations
{
    abstract class BaseOperation : IOperation
    {
        protected ODatabase _database;
        protected OperationType _operationType;

        public BaseOperation(ODatabase database)
        {
            _database = database;
        }

        public IRecordSerializer Serializer
        {
            get { return RecordSerializerFactory.GetSerializer(_database); }
        }

        public virtual Request Request(Request request)
        {
            request.AddDataItem((byte)_operationType);
            request.AddDataItem(request.SessionId);

            if (OClient.ProtocolVersion > 26 && request.Connection.UseTokenBasedSession)
            {
                request.AddDataItem(request.Connection.Token);
            }

            return request;
        }

        public abstract ODocument Response(Response response);

        internal byte[] ReadToken(BinaryReader reader)
        {
            var size = reader.ReadInt32EndianAware();
            var token = reader.ReadBytesRequired(size);
            
            // if token renewed
            if (token.Length > 0)
                _database.GetConnection().Token = token;

            return token;
        }
        protected bool EndOfStream(BinaryReader reader)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var length = (Int32)typeof(NetworkStream).GetTypeInfo().GetField("_readLen", flags).GetValue(reader.BaseStream);
            var pos = (Int32)typeof(NetworkStream).GetTypeInfo().GetField("_readPos", flags).GetValue(reader.BaseStream);
            return length == pos;
        }
    }
}
