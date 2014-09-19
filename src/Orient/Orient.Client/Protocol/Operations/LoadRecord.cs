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

        public LoadRecord(ORID orid)
        {
            _orid = orid;
        }

        public Request Request(int sessionID)
        {
            Request request = new Request();

            // standard request fields
            request.DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray((byte)OperationType.RECORD_LOAD) });
            request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(sessionID) });
            request.DataItems.Add(new RequestDataItem() { Type = "short", Data = BinarySerializer.ToArray(_orid.ClusterId) });
            request.DataItems.Add(new RequestDataItem() { Type = "long", Data = BinarySerializer.ToArray(_orid.ClusterPosition) });
            request.DataItems.Add(new RequestDataItem() {Type = "string", Data = BinarySerializer.ToArray("")} );
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
                byte payload_status = reader.ReadByte();
                if (payload_status == 0)
                {
#if DEBUG
                    if (iRead == 0)
                        DumpRemaining(reader);
#endif
                    break;
                }

                if (iRead > 0)
                    throw new NotImplementedException();

                responseDocument.SetField("PayloadStatus", PayloadStatus.SingleRecord);

                var contentLength = reader.ReadInt32EndianAware();
                byte[] readBytes = reader.ReadBytes(contentLength);
                var version = reader.ReadInt32EndianAware();
                var recordType = reader.ReadByte();
                string serialized = System.Text.Encoding.Default.GetString(readBytes);
                var document = RecordSerializer.Deserialize(serialized);
                document.ORID = _orid;
                //document.OVersion = version;
                //document.OType = type;
                //document.OClassId = classId;
                responseDocument.SetField("Content", document);
                iRead++;
            }

            return responseDocument;
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
