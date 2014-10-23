using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class DBList : IOperation
    {
        public Request Request(Request request)
        {
            request.AddDataItem((byte)OperationType.DB_LIST);
            request.AddDataItem(request.SessionId);
            return request;
        }

        public ODocument Response(Response response)
        {
            ODocument document = new ODocument();
            if (response == null)
            {
                return document;
            }
            var reader = response.Reader;
            int recordLength = reader.ReadInt32EndianAware();
            byte[] rawRecord = reader.ReadBytes(recordLength);
            document = RecordSerializerFactory.GetSerializer(OClient.Serializer).Deserialize(rawRecord, document);
            return document;
        }
    }
}
