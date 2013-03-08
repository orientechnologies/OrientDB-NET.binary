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
                while (payloadStatus != PayloadStatus.NoRemainingRecords)
                {
                    switch (payloadStatus)
                    {
                        case PayloadStatus.ResultSet:
                            // TODO:
                            /*int contentLength = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                            offset += 4;
                            // HACK:
                            string record = BinarySerializer.ToString(response.Data.Skip(offset).Take(contentLength).ToArray());
                            offset += contentLength;

                            string version = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray()).ToString();
                            offset += 4;

                            string type = BinarySerializer.ToByte(response.Data.Skip(offset).Take(1).ToArray()).ToString();
                            offset += 1;

                            content.Add(record + ";" + version + ";" + type);*/
                            break;
                        case PayloadStatus.PreFetched:
                            // TODO:
                            break;
                        default:
                            break;
                    }

                    payloadStatus = (PayloadStatus)BinarySerializer.ToByte(response.Data.Skip(offset).Take(1).ToArray());
                    offset += 1;
                }
            }
            else
            {
                int contentLength;

                switch (payloadStatus)
                {
                    case PayloadStatus.NullResult: // 'n'
                        // TODO:
                        break;
                    case PayloadStatus.SingleRecord: // 'r'
                        // TODO:
                        /*contentLength = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                        offset += 4;
                        // HACK:
                        string record = BinarySerializer.ToString(response.Data.Skip(offset).Take(contentLength).ToArray());
                        offset += contentLength;

                        string version = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray()).ToString();
                        offset += 4;

                        string type = BinarySerializer.ToByte(response.Data.Skip(offset).Take(1).ToArray()).ToString();
                        offset += 1;

                        content.Add(record + ";" + version + ";" + type);*/
                        break;
                    case PayloadStatus.SerializedResult: // 'a'
                        // TODO:
                        /*contentLength = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                        offset += 4;
                        string serialized = BinarySerializer.ToString(response.Data.Skip(offset).Take(contentLength).ToArray());
                        offset += contentLength;

                        content.Add(serialized);*/
                        break;
                    case PayloadStatus.RecordCollection: // 'l'
                        int recordsCount = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                        offset += 4;

                        List<ORecord> records = new List<ORecord>();

                        for (int i = 0; i < recordsCount; i++)
                        {
                            short classId = BinarySerializer.ToShort(response.Data.Skip(offset).Take(2).ToArray());
                            offset += 2;

                            if (classId == -2) // NULL
                            {
                            }
                            else if (classId == -3) // record id
                            {
                            }
                            else
                            {
                                ORecordType type = (ORecordType)BinarySerializer.ToByte(response.Data.Skip(offset).Take(1).ToArray());
                                offset += 1;

                                ORID orid = new ORID();
                                orid.ClusterId = BinarySerializer.ToShort(response.Data.Skip(offset).Take(2).ToArray());
                                offset += 2;

                                orid.ClusterPosition = BinarySerializer.ToLong(response.Data.Skip(offset).Take(8).ToArray());
                                offset += 8;

                                int version = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                                offset += 4;

                                int recordLength = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                                offset += 4;

                                byte[] rawRecord = response.Data.Skip(offset).Take(recordLength).ToArray();
                                offset += recordLength;

                                ORecord record = RecordSerializer.ToRecord(orid, version, type, classId, rawRecord);

                                records.Add(record);
                            }
                        }

                        dataObject.Set("Content", records);
                        break;
                    default:
                        break;
                }
            }
            
            return dataObject;
        }
    }
}
