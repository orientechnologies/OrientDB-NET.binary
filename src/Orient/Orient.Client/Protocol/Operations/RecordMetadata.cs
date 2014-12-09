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
        public RecordMetadata(ODatabase database)
            : base(database)
        {

        }
        public ORID _orid { get; set; }

        public RecordMetadata(ORID rid)
            :base(null)
        {
            _orid = rid;
        }
        public override Request Request(Request request)
        {

            request.AddDataItem((byte)OperationType.RECORD_METADATA);
            request.AddDataItem(request.SessionId);

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
