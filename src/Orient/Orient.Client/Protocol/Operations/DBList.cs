using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class DBList : IOperation
    {
        public Request Request(int sessionID)
        {
            Request request = new Request();
            request.AddDataItem((byte)OperationType.DB_LIST);
            request.AddDataItem(sessionID);
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
            document = RecordSerializer.Deserialize(BinarySerializer.ToString(rawRecord).Trim(), document);
            return document;
        }
    }
}
