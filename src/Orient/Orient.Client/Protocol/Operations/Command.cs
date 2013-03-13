using System.Linq;
using System.Collections.Generic;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class Command : IOperation
    {
        internal OperationMode OperationMode { get; set; }
        internal CommandClassType ClassType { get; set; }
        internal CommandPayload CommandPayload { get; set; }

        public Request Request(int sessionId)
        {
            Request request = new Request();
            
            // standard request fields
            request.DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray((byte)OperationType.COMMAND) });
            request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(sessionId) });
            // operation specific fields
            request.DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray((byte)OperationMode) });

            // class name field
            string className = "x";
            switch (ClassType)
            {
                // idempotent command (e.g. select)
                case CommandClassType.Idempotent:
                    className = "com.orientechnologies.orient.core.sql.query.OSQLSynchQuery";
                    break;
                // non-idempotent command (e.g. insert)
                case CommandClassType.NonIdempotent:
                    className = "com.orientechnologies.orient.core.sql.OCommandSQL";
                    break;
                // script command
                case CommandClassType.Script:
                    className = "com.orientechnologies.orient.core.command.script.OCommandScript";
                    break;
                default:
                    break;
            }

            // TODO: sql script case length
            request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(
                //4 + // this int
                4 + // class name int length
                className.Length + 
                4 + // limit int length
                4 + // text int length
                CommandPayload.Text.Length + 
                4 + // fetch plant int length
                CommandPayload.FetchPlan.Length +
                4 // serialized params int (disable)
            ) });
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(className) });

            if (CommandPayload.Type == CommandPayloadType.SqlScript)
            {
                request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(CommandPayload.Language) });
            }

            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(CommandPayload.Text) });
            request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(CommandPayload.NonTextLimit) });
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(CommandPayload.FetchPlan) });
            //request.DataItems.Add(new RequestDataItem() { Type = "bytes", Data = CommandPayload.SerializedParams });
            // HACK: 0:int means disable
            request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(0) });
            
            return request;
        }

        public DataObject Response(Response response)
        {
            // start from this position since standard fields (status, session ID) has been already parsed
            int offset = 5;
            DataObject dataObject = new DataObject();
            
            if (response == null)
            {
                return dataObject;
            }

            // operation specific fields
            PayloadStatus payloadStatus = (PayloadStatus)BinarySerializer.ToByte(response.Data.Skip(offset).Take(1).ToArray());
            dataObject.Set("PayloadStatus", payloadStatus);
            offset += 1;

            if (OperationMode == OperationMode.Asynchronous)
            {
                List<ORecord> records = new List<ORecord>();

                while (payloadStatus != PayloadStatus.NoRemainingRecords)
                {
                    ORecord record = ParseRecord(ref offset, response.Data);

                    switch (payloadStatus)
                    {
                        case PayloadStatus.ResultSet:
                            records.Add(record);
                            break;
                        case PayloadStatus.PreFetched:
                            // TODO: client cache
                            records.Add(record);
                            break;
                        default:
                            break;
                    }

                    payloadStatus = (PayloadStatus)BinarySerializer.ToByte(response.Data.Skip(offset).Take(1).ToArray());
                    offset += 1;
                }

                dataObject.Set("Content", records);
            }
            else
            {
                int contentLength;

                switch (payloadStatus)
                {
                    case PayloadStatus.NullResult: // 'n'
                        // nothing to do
                        break;
                    case PayloadStatus.SingleRecord: // 'r'
                        ORecord record = ParseRecord(ref offset, response.Data);
                        dataObject.Set("Content", record);
                        break;
                    case PayloadStatus.SerializedResult: // 'a'
                        // TODO: how to handle result
                        contentLength = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                        offset += 4;
                        string serialized = BinarySerializer.ToString(response.Data.Skip(offset).Take(contentLength).ToArray());
                        offset += contentLength;

                        dataObject.Set("Content", serialized);
                        break;
                    case PayloadStatus.RecordCollection: // 'l'
                        int recordsCount = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                        offset += 4;

                        List<ORecord> records = new List<ORecord>();

                        for (int i = 0; i < recordsCount; i++)
                        {
                            records.Add(ParseRecord(ref offset, response.Data));
                        }

                        dataObject.Set("Content", records);
                        break;
                    default:
                        break;
                }
            }
            
            return dataObject;
        }

        private ORecord ParseRecord(ref int offset, byte[] data)
        {
            ORecord record = null;
            short classId = BinarySerializer.ToShort(data.Skip(offset).Take(2).ToArray());
            offset += 2;

            if (classId == -2) // NULL
            {
            }
            else if (classId == -3) // record id
            {
                ORID orid = new ORID();
                orid.ClusterId = BinarySerializer.ToShort(data.Skip(offset).Take(2).ToArray());
                offset += 2;

                orid.ClusterPosition = BinarySerializer.ToLong(data.Skip(offset).Take(8).ToArray());
                offset += 8;

                record = new ORecord();
                record.ORID = orid;
            }
            else
            {
                ORecordType type = (ORecordType)BinarySerializer.ToByte(data.Skip(offset).Take(1).ToArray());
                offset += 1;

                ORID orid = new ORID();
                orid.ClusterId = BinarySerializer.ToShort(data.Skip(offset).Take(2).ToArray());
                offset += 2;

                orid.ClusterPosition = BinarySerializer.ToLong(data.Skip(offset).Take(8).ToArray());
                offset += 8;

                int version = BinarySerializer.ToInt(data.Skip(offset).Take(4).ToArray());
                offset += 4;

                int recordLength = BinarySerializer.ToInt(data.Skip(offset).Take(4).ToArray());
                offset += 4;

                byte[] rawRecord = data.Skip(offset).Take(recordLength).ToArray();
                offset += recordLength;

                record = RecordSerializer.ToRecord(orid, version, type, classId, rawRecord);
            }

            return record;
        }
    }
}
