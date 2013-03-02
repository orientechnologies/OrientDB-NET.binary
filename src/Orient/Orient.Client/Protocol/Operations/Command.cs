using System.Linq;
using System.Collections.Generic;
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
                    //className = "q";
                    //className = "com.orientechnologies.orient.core.sql.query.OSQLSynchQuery";
                    className = "com.orientechnologies.orient.core.sql.query.OSQLSynchQuery";
                    break;
                // non-idempotent command (e.g. insert)
                case CommandClassType.NonIdempotent:
                    className = "c";
                    break;
                // script command
                case CommandClassType.Script:
                    className = "s";
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

        public ResponseDataObject Response(Response response)
        {
            // start from this position since standard fields (status, session ID) has been already parsed
            int offset = 5;
            ResponseDataObject dataObject = new ResponseDataObject();
            
            if (response == null)
            {
                return dataObject;
            }

            // operation specific fields
            byte foo = BinarySerializer.ToByte(response.Data.Skip(offset).Take(1).ToArray());
            PayloadStatus payloadStatus = (PayloadStatus)foo;
            dataObject.Set("PayloadStatus", payloadStatus);
            offset += 1;

            List<string> content = new List<string>();

            if (OperationMode == OperationMode.Asynchronous)
            {
                while (payloadStatus != PayloadStatus.NoRemainingRecords)
                {
                    switch (payloadStatus)
                    {
                        case PayloadStatus.ResultSet:
                            int contentLength = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                            offset += 4;
                            // HACK:
                            string record = BinarySerializer.ToString(response.Data.Skip(offset).Take(contentLength).ToArray());
                            offset += contentLength;

                            string version = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray()).ToString();
                            offset += 4;

                            string type = BinarySerializer.ToByte(response.Data.Skip(offset).Take(1).ToArray()).ToString();
                            offset += 1;

                            content.Add(record + ";" + version + ";" + type);
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
                    case PayloadStatus.NullResult:
                        // TODO:
                        break;
                    case PayloadStatus.SingleRecord:
                        contentLength = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                        offset += 4;
                        // HACK:
                        string record = BinarySerializer.ToString(response.Data.Skip(offset).Take(contentLength).ToArray());
                        offset += contentLength;

                        string version = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray()).ToString();
                        offset += 4;

                        string type = BinarySerializer.ToByte(response.Data.Skip(offset).Take(1).ToArray()).ToString();
                        offset += 1;

                        content.Add(record + ";" + version + ";" + type);
                        break;
                    case PayloadStatus.SerializedResult:
                        contentLength = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                        offset += 4;
                        string serialized = BinarySerializer.ToString(response.Data.Skip(offset).Take(contentLength).ToArray());
                        offset += contentLength;

                        content.Add(serialized);
                        break;
                    case PayloadStatus.RecordCollection:
                        int recordsCount = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                        offset += 4;

                        for (int i = 0; i < recordsCount; i++)
                        {
                            string classId = BinarySerializer.ToShort(response.Data.Skip(offset).Take(2).ToArray()).ToString();
                            offset += 2;

                            string wut = BinarySerializer.ToByte(response.Data.Skip(offset).Take(1).ToArray()).ToString();
                            offset += 1;

                            string clusterId = BinarySerializer.ToShort(response.Data.Skip(offset).Take(2).ToArray()).ToString();
                            offset += 2;

                            string clusterPosition = BinarySerializer.ToLong(response.Data.Skip(offset).Take(8).ToArray()).ToString();
                            offset += 8;

                            string wut2 = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray()).ToString();
                            offset += 4;

                            int recordLength = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                            offset += 4;

                            string recordContent = BinarySerializer.ToString(response.Data.Skip(offset).Take(recordLength).ToArray());
                            offset += recordLength;

                            /*contentLength = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                            offset += 4;
                            string record2 = BinarySerializer.ToString(response.Data.Skip(offset).Take(contentLength).ToArray());
                            offset += contentLength;

                            string version2 = BinarySerializer.ToInt(response.Data.Skip(offset).Take(4).ToArray()).ToString();
                            offset += 4;

                            string type2 = BinarySerializer.ToByte(response.Data.Skip(offset).Take(1).ToArray()).ToString();
                            offset += 1;*/

                            content.Add(string.Format("{0}:{1}:{2}:{3}:{4}:{5}", classId, wut, clusterId, clusterPosition, wut2, recordContent));
                        }
                        break;
                    default:
                        break;
                }
            }

            dataObject.Set("Content", content);
            
            return dataObject;
        }
    }
}
