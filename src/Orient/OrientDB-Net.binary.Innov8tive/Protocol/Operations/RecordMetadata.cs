using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class RecordMetadata : BaseOperation
    {
        public ORID _orid { get; set; }

        public RecordMetadata(ORID rid, ODatabase database)
            : base(database)
        {
            _orid = rid;
            _operationType = OperationType.RECORD_METADATA;
        }
        public override Request Request(Request request)
        {
            base.Request(request);

            request.AddDataItem((short)_orid.ClusterId);
            request.AddDataItem((long)_orid.ClusterPosition);

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

            document.ORID = ReadORID(reader);
            document.OVersion = reader.ReadInt32EndianAware();

            return document;
        }

        private ORID ReadORID(BinaryReader reader)
        {
            ORID result = new ORID();
            result.ClusterId = reader.ReadInt16EndianAware();
            result.ClusterPosition = reader.ReadInt64EndianAware();
            return result;
        }

    }
}
