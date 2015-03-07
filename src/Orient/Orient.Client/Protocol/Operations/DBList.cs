using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class DBList : BaseOperation
    {
        public DBList(ODatabase database)
            : base(database)
        {
            _operationType = OperationType.DB_LIST;
        }
        public override Request Request(Request request)
        {
            base.Request(request);

            return request;
        }

        public override ODocument Response(Response response)
        {
            ODocument document = new ODocument();
            if (response == null)
            {
                return document;
            }

            var reader = response.Reader;
            if (response.Connection.ProtocolVersion > 26 && response.Connection.UseTokenBasedSession)
                ReadToken(reader);

            int recordLength = reader.ReadInt32EndianAware();
            byte[] rawRecord = reader.ReadBytes(recordLength);
            document = Serializer.Deserialize(rawRecord, document);
            return document;
        }
    }
}
