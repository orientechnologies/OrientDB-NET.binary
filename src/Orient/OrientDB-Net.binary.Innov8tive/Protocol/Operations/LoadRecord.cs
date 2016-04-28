using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    class LoadRecord : BaseOperation
    {
        public LoadRecord(ODatabase database)
            : base(database)
        {
            _operationType = OperationType.RECORD_LOAD;
        }
        private readonly ORID _orid;
        private readonly string _fetchPlan;

        public LoadRecord(ORID orid, string fetchPlan, ODatabase database)
            : this(database)
        {
            _orid = orid;
            _fetchPlan = fetchPlan;
            _database = database;
        }

        public override Request Request(Request request)
        {
            base.Request(request);

            request.AddDataItem(_orid);
            request.AddDataItem(_fetchPlan);

            if (OClient.ProtocolVersion >= 9) // Ignore Cache 1-true, 0-false
                request.AddDataItem((byte)0);
            if (OClient.ProtocolVersion >= 13) // Load tombstones 1-true , 0-false
                request.AddDataItem((byte)0);
            return request;
        }

        public override ODocument Response(Response response)
        {
            ODocument responseDocument = new ODocument();

            if (response == null)
            {
                return responseDocument;
            }

            var reader = response.Reader;
            if (response.Connection.ProtocolVersion > 26 && response.Connection.UseTokenBasedSession)
                ReadToken(reader);

            while (true)
            {
                PayloadStatus payload_status = (PayloadStatus)reader.ReadByte();

                bool done = false;
                switch (payload_status)
                {
                    case PayloadStatus.NoRemainingRecords:
                        done = true;
                        break;
                    case PayloadStatus.ResultSet:
                        ReadPrimaryResult(responseDocument, reader);
                        break;
                    case PayloadStatus.PreFetched:
                        ReadAssociatedResult(reader);
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                if (done)
                {
                    break;
                }
            }

            return responseDocument;
        }

        private void ReadAssociatedResult(BinaryReader reader)
        {
            var zero = reader.ReadInt16EndianAware();
            if (zero != 0)
                throw new InvalidOperationException("Unsupported record format");

            ORecordType recordType = (ORecordType)reader.ReadByte();
            if (recordType != ORecordType.Document)
                throw new InvalidOperationException("Unsupported record type");

            short clusterId = reader.ReadInt16EndianAware();
            long clusterPosition = reader.ReadInt64EndianAware();
            int recordVersion = reader.ReadInt32EndianAware();

            var recordLength = reader.ReadInt32EndianAware();
            var record = reader.ReadBytes(recordLength);

            //var document = RecordCSVSerializer.Deserialize(new ORID(clusterId, clusterPosition), recordVersion, ORecordType.Document, 0, record);
            var document = Serializer
                .Deserialize(record, new ODocument { ORID = new ORID(clusterId, clusterPosition), OVersion = recordVersion, OType = ORecordType.Document });

            _database.ClientCache[document.ORID] = document;
        }

        private void ReadPrimaryResult(ODocument responseDocument, BinaryReader reader)
        {
            responseDocument.SetField("PayloadStatus", PayloadStatus.SingleRecord);
            int contentLength;
            byte[] readBytes;
            int version;
            ORecordType recordType;

            if (OClient.ProtocolVersion < 28)
            {
                contentLength = reader.ReadInt32EndianAware();
                readBytes = reader.ReadBytes(contentLength);
                version = reader.ReadInt32EndianAware();
                recordType = (ORecordType)reader.ReadByte();
            }
            else
            {
                recordType = (ORecordType)reader.ReadByte();
                version = reader.ReadInt32EndianAware();
                contentLength = reader.ReadInt32EndianAware();
                readBytes = reader.ReadBytes(contentLength);
            }

            var document = new ODocument();

            switch (recordType)
            {
                case ORecordType.Document:
                    document = Serializer.Deserialize(readBytes, new ODocument());
                    document.ORID = _orid;
                    document.OVersion = version;
                    responseDocument.SetField("Content", document);
                    break;
                case ORecordType.RawBytes:
                    document.SetField("RawBytes", readBytes);
                    responseDocument.SetField("Content", document);
                    break;
                case ORecordType.FlatData:
                    break;
            }
        }

    }
}
