using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    class LoadRecord : IOperation
    {
        private readonly ORID _orid;
        private readonly string _fetchPlan;
        private readonly ODatabase _database;

        public LoadRecord(ORID orid, string fetchPlan, ODatabase database)
        {
            _orid = orid;
            _fetchPlan = fetchPlan;
            _database = database;
        }

        public Request Request(int sessionID)
        {
            Request request = new Request();

            // standard request fields
            request.DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray((byte)OperationType.RECORD_LOAD) });
            request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(sessionID) });
            request.DataItems.Add(new RequestDataItem() { Type = "short", Data = BinarySerializer.ToArray(_orid.ClusterId) });
            request.DataItems.Add(new RequestDataItem() { Type = "long", Data = BinarySerializer.ToArray(_orid.ClusterPosition) });
            request.DataItems.Add(new RequestDataItem() {Type = "string", Data = BinarySerializer.ToArray(_fetchPlan)} );
            request.DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray((byte)0) });
            request.DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray((byte)0) });

            return request;
        }

        public ODocument Response(Response response)
        {
            ODocument responseDocument = new ODocument();

            if (response == null)
            {
                return responseDocument;
            }

            var reader = response.Reader;

            int iRead = 0;
            while (true)
            {
                PayloadStatus payload_status = (PayloadStatus) reader.ReadByte();

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
#if DEBUG
                    if (iRead == 0)
                        DumpRemaining(reader);
#endif
                    break;
                }

                iRead++;
            }

            return responseDocument;
        }

        private void ReadAssociatedResult(BinaryReader reader)
        {
            var zero = reader.ReadInt16EndianAware();
            if (zero != 0)
                throw new InvalidOperationException("Unsupported record format");

            byte recordType = reader.ReadByte();
            if (recordType != (byte) 'd')
                throw new InvalidOperationException("Unsupported record type");

            short clusterId = reader.ReadInt16EndianAware();
            long clusterPosition = reader.ReadInt64EndianAware();
            int recordVersion = reader.ReadInt32EndianAware();

            var recordLength = reader.ReadInt32EndianAware();
            var record = reader.ReadBytes(recordLength);

            var document = RecordSerializer.Deserialize(new ORID(clusterId, clusterPosition), recordVersion, ORecordType.Document, 0, record);

            _database.ClientCache[document.ORID] = document;
        }

        private void ReadPrimaryResult(ODocument responseDocument, BinaryReader reader)
        {
            responseDocument.SetField("PayloadStatus", PayloadStatus.SingleRecord);

            var contentLength = reader.ReadInt32EndianAware();
            byte[] readBytes = reader.ReadBytes(contentLength);
            var version = reader.ReadInt32EndianAware();
            var recordType = reader.ReadByte();


            string serialized = System.Text.Encoding.Default.GetString(readBytes);
            var document = RecordSerializer.Deserialize(serialized);
            document.ORID = _orid;
            document.OVersion = version;
            //document.OType = type;
            //document.OClassId = classId;
            responseDocument.SetField("Content", document);
        }

        private void DumpRemaining(BinaryReader reader)
        {
            int i = 0;
            while (true)
            {
                i++;
                var b = reader.ReadByte();
                Trace.WriteLine(string.Format( "{2} {0:X02} {1},", b, (char)b, i));
            }
        }
    }
}
